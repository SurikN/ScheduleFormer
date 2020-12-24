using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ScheduleFormer.Annotations;
using ScheduleFormer.Containers;
using ScheduleFormer.Models;

namespace ScheduleFormer.ViewModels
{
    public class AddLecturesViewModel : INotifyPropertyChanged
    {
        private List<Lecture> _lectures = new List<Lecture>();

        public ObservableCollection<Group> Audiences { get; set; } = new ObservableCollection<Group>();

        public ObservableCollection<Teacher> Lecturers { get; set; } = new ObservableCollection<Teacher>();

        public ObservableCollection<Lecture> Lectures { get; set; } = new ObservableCollection<Lecture>();

        public string SelectedName { get; set; }

        public string SelectedAudience { get; set; }

        public string SelectedLecturer { get; set; }

        public string SelectedQuantity { get; set; }

        public Lecture SelectedLecture { get; set; }

        public ICommand AddLectureCommand => new RelayCommand(OnAddLectureCommand, true);

        public ICommand RemoveLectureCommand => new RelayCommand(OnRemoveLectureCommand, CanRemoveLectureCommand);

        public ICommand OkCommand => new GalaSoft.MvvmLight.CommandWpf.RelayCommand<Window>(OnOkCommand, true);

        private void OnAddLectureCommand()
        {
            UpdateLists();
            if (!int.TryParse(SelectedQuantity, out var quantity) || quantity <= 0)
            {
                throw new ApplicationException();
            }
            Lectures.Add(new Lecture
            {
                Audience = new Group(SelectedAudience),
                Lecturer = new Teacher(SelectedLecturer),
                Name = SelectedName
            });
            for (var i = 0; i < quantity; i++)
            {
                _lectures.Add(new Lecture
                {
                    Audience = new Group(SelectedAudience),
                    Lecturer = new Teacher(SelectedLecturer),
                    Name = SelectedName
                });
            }

            Clear();
        }

        private void OnRemoveLectureCommand()
        {
            while (Lectures.Any(a => a.Equals(SelectedLecture)))
            {
                Lectures.Remove(SelectedLecture);
            }

            SelectedLecture = null;
            OnPropertyChanged(nameof(SelectedLecture));
        }

        private void OnOkCommand(Window obj)
        {
            LecturesStorageModel.Lectures.Clear();
            foreach (var lecture in _lectures)
            {
                LecturesStorageModel.Add(lecture);
            }
            ((Window)obj).Close();
        }

        private bool CanAddLectureCommand()
        {
            return !string.IsNullOrWhiteSpace(SelectedAudience) && !string.IsNullOrWhiteSpace(SelectedLecturer) &&
                   !string.IsNullOrWhiteSpace(SelectedName) && int.TryParse(SelectedQuantity, out var quantity) && quantity <= 0;
        }

        private bool CanRemoveLectureCommand()
        {
            return SelectedLecture != null;
        }

        private void UpdateLists()
        {
            if (Audiences.All(a => a.Name != SelectedAudience))
            {
                Audiences.Add(new Group(SelectedAudience));
            }
            if (Lecturers.All(a => a.Name != SelectedLecturer))
            {
                Lecturers.Add(new Teacher(SelectedLecturer));
            }
        }

        private void Clear()
        {
            SelectedAudience = string.Empty;
            SelectedLecturer = string.Empty;
            SelectedName = string.Empty;
            SelectedQuantity = string.Empty;

            OnPropertyChanged(nameof(SelectedAudience));
            OnPropertyChanged(nameof(SelectedLecturer));
            OnPropertyChanged(nameof(SelectedName));
            OnPropertyChanged(nameof(SelectedQuantity));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

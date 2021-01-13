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
        private string _selectedLecturer, _selectedAudience, _selectedName, _selectedQuantity;

        private Lecture _savedSelectedLecture;

        private Lecture _selectedLecture;

        private bool _isEditing = false;

        private List<Lecture> _lectures;

        public ObservableCollection<Group> Audiences { get; set; } = new ObservableCollection<Group>();

        public ObservableCollection<Teacher> Lecturers { get; set; } = new ObservableCollection<Teacher>();

        public ObservableCollection<Lecture> Lectures { get; set; } = new ObservableCollection<Lecture>();

        public AddLecturesViewModel()
        {
            _lectures = new List<Lecture>(LecturesStorageModel.Lectures);
            UpdateLists();
            UpdateLectures();
        }

        public bool IsNotEditing
        {
            get => !_isEditing;
            set => IsEditing = !value;
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotEditing));
            }
        }

        public string SelectedName
        {
            get => _selectedName;
            set
            {
                _selectedName = value;
                OnPropertyChanged();
            }
        }

        public string SelectedAudience
        {
            get => _selectedAudience;
            set
            {
                _selectedAudience = value;
                OnPropertyChanged();
            }
        }

        public string SelectedLecturer
        {
            get => _selectedLecturer;
            set
            {
                _selectedLecturer = value;
                OnPropertyChanged();
            }
        }

        public string SelectedQuantity
        {
            get => _selectedQuantity;
            set
            {
                _selectedQuantity = value;
                OnPropertyChanged();
            }
        }

        public Lecture SelectedLecture
        {
            get => _selectedLecture;
            set
            {
                _selectedLecture = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddLectureCommand => new RelayCommand(OnAddLectureCommand, true);

        public ICommand RemoveLectureCommand => new RelayCommand(OnRemoveLectureCommand, SelectedLecture != null);

        public ICommand OkCommand => new RelayCommand<Window>(OnOkCommand, true);

        public ICommand EditCommand => new RelayCommand(OnEditCommand, SelectedLecture != null);

        public ICommand ConfirmEditCommand => new RelayCommand(OnConfirmEditCommand, _isEditing);

        public ICommand CancelEditCommand => new RelayCommand(OnCancelEditCommand, _isEditing);

        private void OnConfirmEditCommand()
        {
            RemoveSelectedLecture();
            OnAddLectureCommand();
            //int diff;

            ////Quantity alignment
            //if ((diff = LecturesCount(_savedSelectedLecture) - int.Parse(SelectedQuantity)) > 0)
            //{
            //    for (var i = 0; i < diff; i++)
            //    {
            //        _lectures.Remove(_lectures.First(a=> a.Equals(_savedSelectedLecture)));
            //    }
            //}
            //else
            //{
            //    for (var i = 0; i > diff; i--)
            //    {
            //        _lectures.Add(new Lecture(_savedSelectedLecture));
            //    }
            //}

            ////Data alignment
            //foreach (var lecture in _lectures)
            //{
            //    if (!lecture.Equals(_savedSelectedLecture)) continue;
            //    lecture.Audience = new Group(SelectedAudience);
            //    lecture.Lecturer = new Teacher(SelectedLecturer);
            //    lecture.Name = SelectedName;
            //}

            //UpdateLists();
            //UpdateLectures();
            //IsNotEditing = true;
        }

        private void OnCancelEditCommand()
        {
            Clear();
            IsNotEditing = true;
        }

        private int LecturesCount(Lecture lecture)
        {
            return _lectures.Count(a =>
                a.Name == lecture.Name && a.Lecturer.ToString() == lecture.Lecturer.ToString() &&
                a.Audience.ToString() == lecture.Audience.ToString());
        }

        private void OnEditCommand()
        {
            SelectedAudience = SelectedLecture.Audience.ToString();
            SelectedLecturer = SelectedLecture.Lecturer.ToString();
            SelectedName = SelectedLecture.Name;
            SelectedQuantity = LecturesCount(SelectedLecture).ToString();

            _savedSelectedLecture = SelectedLecture;
            IsEditing = true;
        }

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

        private void UpdateLectures()
        {
            Lectures.Clear();
            foreach (var lecture in _lectures)
            {
                if (!Lectures.Any(a=> a.Equals(lecture)))
                {
                    Lectures.Add(lecture);
                }
            }
        }

        private void OnRemoveLectureCommand()
        {
            if (MessageBox.Show("Are you sure you want to delete lectures?", "Confirm action", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            _savedSelectedLecture = SelectedLecture;
            RemoveSelectedLecture();
        }

        public void RemoveSelectedLecture()
        {
            var toDelete = new List<Lecture>();
            foreach (var lecture in _lectures)
            {
                if (lecture.Equals(_savedSelectedLecture))
                {
                    toDelete.Add(lecture);
                }
            }

            foreach (var lecture in toDelete)
            {
                _lectures.Remove(lecture);
            }
            UpdateLectures();
            OnPropertyChanged(nameof(Lectures));
            SelectedLecture = null;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

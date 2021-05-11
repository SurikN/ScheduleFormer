using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private LecturesStorageModel _lecturesStorageModel = LecturesStorageModel.GetInstance();

        public ObservableCollection<Group> Audiences { get; set; } = new ObservableCollection<Group>();

        public ObservableCollection<Teacher> Lecturers { get; set; } = new ObservableCollection<Teacher>();

        public ObservableCollection<CombinedLecture> Lectures { get; set; } = new ObservableCollection<CombinedLecture>();

        public AddLecturesViewModel()
        {
            foreach (var uniqueGroup in _lecturesStorageModel.UniqueGroups)
            {
                Audiences.Add(uniqueGroup);
            }

            foreach (var uniqueTeacher in _lecturesStorageModel.UniqueTeachers)
            {
                Lecturers.Add(uniqueTeacher);
            }

            foreach (var combinedLecture in _lecturesStorageModel.CombinedLectures)
            {
                Lectures.Add(combinedLecture);
            }
            _lectures = new List<Lecture>(_lecturesStorageModel.Lectures);

            if (_lecturesStorageModel.CombinedLectures != null && _lecturesStorageModel.CombinedLectures.Count > 0)
            {
                FillCombinedLectures();
            }

            UpdateLists();
            UpdateLectures();
        }

        private void FillCombinedLectures()
        {
            foreach (var combinedLecture in _lecturesStorageModel.CombinedLectures)
            {
                SelectedName = combinedLecture.Name;
                SelectedAudience = combinedLecture.Audience.Name;
                SelectedLecturer = combinedLecture.Lecturer.Name;
                SelectedQuantity = combinedLecture.Quantity.ToString();
                OnAddLectureCommand();
            }
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

            if (Lecturers.All(a => a.Name != SelectedLecturer))
            {
                Lecturers.Add(new Teacher(SelectedLecturer));
            }

            if (Audiences.All(a => a.Name != SelectedAudience))
            {
                Audiences.Add(new Group(SelectedAudience));
            }

            for (var i = 0; i < quantity; i++)
            {
                _lectures.Add(new Lecture
                {
                    Audience = Audiences.First(a => a.Name == SelectedAudience),
                    Lecturer = Lecturers.First(a => a.Name == SelectedLecturer),
                    Name = SelectedName
                });
            }

            Lectures.Add(new CombinedLecture(new Lecture()
            {
                Audience = Audiences.First(a => a.Name == SelectedAudience),
                Lecturer = Lecturers.First(a => a.Name == SelectedLecturer),
                Name = SelectedName
            }, quantity));

            Clear();
        }

        private void UpdateLectures()
        {
            Lectures.Clear();
            foreach (var lecture in _lectures)
            {
                if (!Lectures.Any(a => a.Equals(lecture)))
                {
                    Lectures.Add(new CombinedLecture(lecture, _lectures.Count(a => a.Equals(lecture))));
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
            for (int i = 0; i < _lectures.Count; i++)
            {
                if (_lectures[i].Name == _savedSelectedLecture.Name && _lectures[i].Audience.Equals(_savedSelectedLecture.Audience) && _lectures[i].Lecturer.Equals(_savedSelectedLecture.Lecturer))
                {
                    toDelete.Add(_lectures[i]);
                }
            }

            foreach (var lecture in toDelete)
            {
                _lectures.Remove(lecture);
            }

            var tempLectures = new List<CombinedLecture>(Lectures);

            foreach (var lecture in tempLectures)
            {
                if (lecture.Name == _savedSelectedLecture.Name && lecture.Audience == _savedSelectedLecture.Audience && lecture.Lecturer == _savedSelectedLecture.Lecturer)
                {
                    Lectures.Remove(lecture);
                }
            }

            UpdateLectures();
            OnPropertyChanged(nameof(Lectures));
            IsNotEditing = true;
            SelectedLecture = null;
        }

        private void OnOkCommand(Window obj)
        {
            _lecturesStorageModel.Lectures.Clear();
            foreach (var lecture in _lectures)
            {
                _lecturesStorageModel.Add(lecture);
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

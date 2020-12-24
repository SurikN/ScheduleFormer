using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using ScheduleFormer.Containers;
using ScheduleFormer.Models;
using ScheduleFormer.Structures;
using ScheduleFormer.Views;

namespace ScheduleFormer.ViewModels
{
    public class MainWindowViewModel
    {
        private GlobalSchedule _globalSchedule;

        public ObservableCollection<Teacher> Lecturers { get; set; }

        public ObservableCollection<Group> Audiences { get; set; }

        public List<Lecture> Lectures { get; set; }
        
        public GlobalSchedule Schedule { get; set; }

        public ICommand AddCommand => new RelayCommand(OnAddCommand, true);

        public ICommand CreateScheduleCommand => new RelayCommand(OnCreateScheduleCommand, true);

        public void AddGroups(IEnumerable<Group> groups)
        {
            foreach (var @group in groups)
            {
                Audiences.Add(group);
            }
        }

        public void AddLecturers(IEnumerable<Teacher> lecturers)
        {
            foreach (var lecturer in lecturers)
            {
                Lecturers.Add(lecturer);
            }
        }

        public void OnCreateScheduleCommand()
        {
            try
            {
                var tempLectures = new List<Lecture>(LecturesStorageModel.Lectures);
                _globalSchedule =
                    new GlobalSchedule(LecturesStorageModel.UniqueGroups, LecturesStorageModel.UniqueTeachers);

                Lecture tempLecture;
                var rng = new Random();
                while (tempLectures.Count > 0)
                {
                    var index = rng.Next(0, tempLectures.Count);
                    tempLecture = tempLectures.ElementAt(index);

                    _globalSchedule.AddLecture(tempLecture.Name, tempLecture.Audience, tempLecture.Lecturer);

                    tempLectures.RemoveAt(index);
                }
            }
            catch
            {
                //ignore
            }
        }

        private void OnAddCommand()
        {
            var view = new AddLecturesView();
            view.Show();
            Lectures = new List<Lecture>(LecturesStorageModel.Lectures);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using ScheduleFormer.Annotations;
using ScheduleFormer.Containers;
using ScheduleFormer.Enums;
using ScheduleFormer.Models;
using ScheduleFormer.Structures;
using ScheduleFormer.Views;

namespace ScheduleFormer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Private fields

        private GlobalSchedule _globalSchedule;

        private DataTable _scheduleDataTable = new DataTable();

        #endregion

        #region Public properties

        public ObservableCollection<Teacher> Lecturers { get; set; }

        public ObservableCollection<Group> Audiences { get; set; }

        #region Days

        public DataTable MondayTable { get; set; }

        public DataTable ScheduleDataTable
        {
            get => _scheduleDataTable;
            set
            {
                _scheduleDataTable = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public List<Lecture> _lectures { get; set; }

        public GlobalSchedule Schedule { get; set; }

        #endregion

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

                FormDataTable();
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
            _lectures = new List<Lecture>(LecturesStorageModel.Lectures);
        }

        private void UpdateDays()
        {

        }

        private void FormDataTable()
        {
            DataTable tempDT = new DataTable();
            DataColumn column;
            DataRow row;
            foreach (var groupSchedule in _globalSchedule.GroupSchedules)
            {
                column = new DataColumn()
                {
                    DataType = typeof(string),
                    ColumnName = groupSchedule.Key
                };
                tempDT.Columns.Add(column);
            }

            for (var i = 0; i < 40; i++)
            {
                row = tempDT.NewRow();
                tempDT.Rows.Add(row);
            }

            try
            {
                foreach (var schedule in _globalSchedule.GroupSchedules)
                {
                    foreach (var day in schedule.Value.Lectures)
                    {
                        foreach (var lecture in day.Value.Lectures)
                        {
                            var mod1 = ((int) day.Key - 1) * 8;
                            var mod2 = ((int) lecture.Key - 1);
                            tempDT.Rows[mod1 + mod2][schedule.Key] =
                                lecture.Value != null ? lecture.Value.ToString() : "-";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var k = e.Message;
            }

            ScheduleDataTable = tempDT;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

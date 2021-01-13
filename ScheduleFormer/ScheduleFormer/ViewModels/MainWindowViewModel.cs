using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
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

        private AddLecturesView _addLecturesView = null;

        private GlobalSchedule _globalSchedule;

        private DataTable _scheduleDataTable = new DataTable();

        private bool _isScheduleCreated = false;

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

        public ICommand EditCommand => new RelayCommand(OnEditCommand, true);

        public ICommand CreateScheduleCommand => new RelayCommand(OnCreateScheduleCommand, true);

        public ICommand ExportCommand => new RelayCommand(OnExportCommand, _isScheduleCreated);

        public ICommand ClearCommand => new RelayCommand(OnClearCommand, true);

        private void OnClearCommand()
        {
            if (MessageBox.Show("Are you sure you want to delete all schedule?", "Confirm action",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            _lectures = new List<Lecture>(LecturesStorageModel.Lectures);
            _scheduleDataTable.Clear();
            _addLecturesView = null;
        }

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
                _isScheduleCreated = false;
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
                _isScheduleCreated = true;
            }
            catch (Exception e)
            {
                var k = e.Message;
            }
        }

        private void OnEditCommand()
        {
            _addLecturesView = new AddLecturesView();
            _addLecturesView.Show();
            //_lectures = new List<Lecture>(LecturesStorageModel.Lectures);
        }

        private void OnExportCommand()
        {
            try
            {
                if (!_isScheduleCreated)
                {
                    MessageBox.Show("Create Schedule first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var saveFileDialog = new SaveFileDialog()
                {
                    DefaultExt = ".csv",
                };
                if (saveFileDialog.ShowDialog() != true) return;
                var dataTable = _scheduleDataTable;

                var lines = new List<string>();

                string[] columnNames = dataTable.Columns
                    .Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .ToArray();

                var header = string.Join(",", columnNames.Select(name => $"\"{name.Replace("\n", ": ")}\""));
                lines.Add(header);

                var valueLines = dataTable.AsEnumerable()
                    .Select(row => string.Join(",", row.ItemArray.Select(val => $"\"{val.ToString().Replace("\n", ": ") }\"")));

                lines.AddRange(valueLines);

                File.WriteAllLines(saveFileDialog.FileName, lines, Encoding.Unicode);
            }
            catch
            {
                MessageBox.Show("Error has occurred during exporting process.", "Error!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void FormDataTable()
        {
            DataTable tempDT = new DataTable();
            tempDT.Columns.Add(new DataColumn()
            {
                DataType = typeof(string),
                ColumnName = "Day of week"
            });
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

                foreach (Days day in typeof(Days).GetEnumValues())
                {
                    var mod1 = 8 * ((int)day - 1);
                    tempDT.Rows[mod1]["Day of week"] = day;
                    foreach (LectureTimes lectureTime in typeof(LectureTimes).GetEnumValues())
                    {
                        tempDT.Rows[mod1 + (int)lectureTime]["Day of week"] = (int)lectureTime;
                    }
                }
                foreach (var schedule in _globalSchedule.GroupSchedules)
                {
                    foreach (var day in schedule.Value.Lectures)
                    {
                        foreach (var lecture in day.Value.Lectures)
                        {
                            var mod1 = ((int)day.Key - 1) * 8;
                            var mod2 = (int)lecture.Key;
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

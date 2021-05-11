using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Spire.Xls;
using Group = ScheduleFormer.Containers.Group;

namespace ScheduleFormer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Private fields

        private AddLecturesView _addLecturesView = new AddLecturesView();

        private GlobalSchedule _globalSchedule;

        private DataTable _scheduleDataTable = new DataTable();

        private bool _isScheduleCreated = false;
        private LecturesStorageModel _lecturesStorageModel = LecturesStorageModel.GetInstance();

        #endregion

        public MainWindowViewModel()
        {
            _globalSchedule = new GlobalSchedule(_lecturesStorageModel);
        }

        #region Public properties

        public ObservableCollection<Teacher> Lecturers { get; set; }

        public ObservableCollection<Group> Audiences { get; set; }

        #region Days

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

        public ICommand ImportCommand => new RelayCommand(OnImportCommand, _isScheduleCreated);

        public ICommand ClearCommand => new RelayCommand(OnClearCommand, true);

        private void OnClearCommand()
        {
            if (MessageBox.Show("Ви впевнені, що хочете видалити весь розклад? Ця дія видалить усі додані пари. Цю дію не можна відмінити.", "Підтвердіть дію",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            _lecturesStorageModel.Lectures.Clear();

            _lectures = new List<Lecture>(_lecturesStorageModel.Lectures);
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
                var counter = CreateSchedule();

                FormDataTable();
                _isScheduleCreated = true;

                MessageBox.Show(counter.ToString(), "Operations", MessageBoxButton.OK);
            }
            catch (Exception e)
            {
                var k = e.Message;
            }
        }

        private int CreateSchedule()
        {
            _isScheduleCreated = false;
            return
            _globalSchedule.CreateScheduleOnLecture();
        }

        private void OnEditCommand()
        {
            _addLecturesView = new AddLecturesView();
            _addLecturesView.Show();
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

                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];

                sheet.Name = "Schedule";

                var dataTable = _scheduleDataTable;

                var i = 1;
                var c = 'A';
                var s = string.Empty;

                string[] columnNames = dataTable.Columns
                    .Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .ToArray();

                foreach (var columnName in columnNames)
                {
                    var columnIndex = s + c;
                    var rowIndex = i;
                    sheet.Range[columnIndex + rowIndex].Text = columnName;
                    if (c < 'Z')
                    {
                        c++;
                    }
                    else
                    {
                        s = "A";
                        c = 'A';
                    }
                }

                s = string.Empty;
                c = 'A';
                i = 2;

                var l = 'A';

                foreach (DataRow dataRow in _scheduleDataTable.Rows)
                {
                    var rowIndex = i;
                    foreach (var columnName in columnNames)
                    {
                        var columnIndex = s + c;
                        sheet[columnIndex + rowIndex].Text = dataRow[columnName].ToString();
                        if (c < 'Z')
                        {
                            c++;
                        }
                        else
                        {
                            s = l.ToString();
                            l++;
                            c = 'A';
                        }
                    }

                    s = string.Empty;
                    l = 'A';
                    c = 'A';
                    i++;
                }

                var toDelete = new List<Worksheet>();

                for (var j = 1; j < wb.Worksheets.Count; j++)
                {
                    toDelete.Add(wb.Worksheets[j]);
                }

                foreach (var worksheet in toDelete)
                {
                    worksheet.Remove();
                }

                wb.SaveToFile(saveFileDialog.FileName);
            }
            catch
            {
                MessageBox.Show("Error has occurred during exporting process.", "Error!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnImportCommand()
        {
            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = ".xlsx"
            };
            if (openFileDialog.ShowDialog() != true) return;

            var path = openFileDialog.FileName;

            var wb = new Workbook();

            wb.LoadFromFile(path);
            var sheet = wb.Worksheets[0];
            var lectures = ExportTable(sheet);
            
            _lecturesStorageModel.CombinedLectures = new List<CombinedLecture>(lectures);
        }

        private List<CombinedLecture> ExportTable(Worksheet sheet)
        {
            var result = new List<CombinedLecture>();
            foreach (var sheetColumn in sheet.Columns)
            {
                var lecture = new Lecture
                {
                    Audience = new Group(sheetColumn.CellList[0].Text),
                    Name = sheetColumn.CellList[1].Text,
                    Lecturer = new Teacher(sheetColumn.CellList[3].Text)
                };
                result.Add(new CombinedLecture(lecture, (int)sheetColumn.CellList[2].NumberValue));
            }

            return result;
        }


        private void FormDataTable()
        {
            DataTable tempDT = new DataTable();
            tempDT.Columns.Add(new DataColumn()
            {
                DataType = typeof(string),
                ColumnName = "День тижня"
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
                    switch (day)
                    {
                        case Days.Monday:
                            tempDT.Rows[mod1]["День тижня"] = "Понеділок";
                            break;
                        case Days.Tuesday:
                            tempDT.Rows[mod1]["День тижня"] = "Вівторок";
                            break;
                        case Days.Wednesday:
                            tempDT.Rows[mod1]["День тижня"] = "Середа";
                            break;
                        case Days.Thursday:
                            tempDT.Rows[mod1]["День тижня"] = "Четвер";
                            break;
                        case Days.Friday:
                            tempDT.Rows[mod1]["День тижня"] = "П'ятниця";
                            break;
                    }
                    //tempDT.Rows[mod1]["День тижня"] = day;
                    foreach (LectureTimes lectureTime in typeof(LectureTimes).GetEnumValues())
                    {
                        tempDT.Rows[mod1 + (int)lectureTime]["День тижня"] = (int)lectureTime;
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

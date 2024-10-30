using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using ReactiveUI;

namespace AvaloniaApplication1.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Person> People { get; } = [new Person("test", "test", true)];
        public HierarchicalTreeDataGridSource<Person> PersonSource { get; }

        public MainWindowViewModel()
        {
            PersonSource = new HierarchicalTreeDataGridSource<Person>(People)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<Person>(
                        new TextColumn<Person, string>
                            ("First Name", x => x.FirstName),
                        x => x.Children),
                    new TextColumn<Person, string>
                        ("First Name", x => x.FirstName),
                    new TextColumn<Person, string>
                        ("Last Name", x => x.LastName),
                    new TextColumn<Person, string>
                        ("Last Name", x => x.ThirdName)
                },
            };

            for (int i = 0; i < 10; i++)
            {
                People.First().Children.Add(new Person("sub", "sub", true));
            }

            People.First().Children.First().Children.Add(new Person("sub", "sub", true));

            IDisposable observable = CreateNewName()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((x) =>
                {
                    //People.First().LastName = x.ToString();
                    //People.First().Children.First().LastName = x.ToString();
                    People.First().Children.First().LastName = x.ToString();
                    People.First().Children.First().ThirdName = x.ToString();
                });
        }

        IObservable<int> CreateNewName()
        {
            return Observable.Create<int>(observable =>
            {
                Task.Run(async () =>
                {
                    int i = 0;

                    while (true)
                    {
                        observable.OnNext(i);
                        i++;

                        await Task.Delay(5000);
                    }
                });

                return Disposable.Empty;
            });
        }
    }

    public class Person : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private string _thirdName;

        public string FirstName
        {
            get => _firstName;
            set => SetField(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetField(ref _lastName, value);
        }

        public string ThirdName
        {
            get => _thirdName;
            set => SetField(ref _thirdName, value);
        }

        public ObservableCollection<Person> Children { get; } = [];

        public bool IsFictitious { get; set; }

        public Person(string firstName, string lastName, bool isFictitious)
        {
            FirstName = firstName;
            LastName = lastName;
            IsFictitious = isFictitious;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
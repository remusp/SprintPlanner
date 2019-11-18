using SprintPlanner.Core.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class UserLoadViewModel : WrappingViewModel<UserLoadModel>
    {
        public UserLoadViewModel(UserLoadModel model) : base(model)
        {
            _issues = new ObservableCollection<IssueViewModel>(model.Issues.Select(i => new IssueViewModel(i)));
            _issues.CollectionChanged += Issues_CollectionChanged;
            _isExpanded = true;
        }

        private void Issues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is List<IssueViewModel> newItemsList)
                    {
                        newItemsList.ForEach(i => _model.Issues.Add(i.GetModel()));
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is List<IssueViewModel> oldItemsList)
                    {
                        oldItemsList.ForEach(i => _model.Issues.Remove(i.GetModel()));
                    }

                    break;

                default:
                    throw new NotImplementedException("Not supported yet!");
            }
        }

        public string Uid
        {
            get { return _model.UserDetails.Uid; }
            set
            {
                _model.UserDetails.Uid = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get { return _model.UserDetails.UserName; }
            set
            {
                _model.UserDetails.UserName = value;
                RaisePropertyChanged();
            }
        }

        public decimal Capacity
        {
            get { return _model.UserDetails.Capacity; }
        }

        public decimal ScaledCapacity
        {
            get { return _model.UserDetails.ScaledCapacity; }
        }

        public decimal CapacityFactor
        {
            get { return _model.UserDetails.CapacityFactor; }
            set
            {
                _model.UserDetails.CapacityFactor = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ScaledCapacity));
            }
        }

        public decimal Load
        {
            get { return _model.Load; }
            set
            {
                _model.Load = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(BookingPercent));
                RaisePropertyChanged(nameof(Availability));
            }
        }

        public decimal BookingPercent
        {
            get
            {
                decimal bp = 0;
                if (_model.UserDetails.Capacity != 0)
                {
                    bp = _model.Load / _model.UserDetails.Capacity;
                }

                return bp;
            }
        }

        public decimal Availability
        {
            get { return ScaledCapacity - _model.Load; }
        }

        private ObservableCollection<IssueViewModel> _issues;

        public ObservableCollection<IssueViewModel> Issues
        {
            get { return _issues; }
            set
            {
                _issues = value;
                RaisePropertyChanged();
            }
        }

        public byte[] PictureData
        {
            get
            {
                return _model.PictureData;
            }

            set
            {
                _model.PictureData = value;
                RaisePropertyChanged();
            }
        }


        private UserStatus _status;

        public UserStatus Status
        {
            get { return _status; }
            set => Set(ref _status, value);
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }


    }
}

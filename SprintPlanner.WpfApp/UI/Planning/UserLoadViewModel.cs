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
            Issues = new ObservableCollection<IssueViewModel>(model.Issues.Select(i => new IssueViewModel(i)));
            Issues.CollectionChanged += Issues_CollectionChanged;
            IsExpanded = true;
        }

        [DependsUpon(nameof(Load))]
        public decimal Availability
        {
            get { return ScaledCapacity - _model.Load; }
        }

        [DependsUpon(nameof(Load))]
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

        public decimal Capacity
        {
            get { return _model.UserDetails.Capacity; }
        }

        public decimal CapacityFactor
        {
            get { return _model.UserDetails.CapacityFactor; }
            set { SetBackingField(() => _model.UserDetails.CapacityFactor = value); }
        }

        public bool IsExpanded
        {
            get { return Get(() => IsExpanded); }
            set { Set(() => IsExpanded, value); }
        }

        public ObservableCollection<IssueViewModel> Issues
        {
            get { return Get(() => Issues); }
            set { Set(() => Issues, value); }
        }

        public decimal Load
        {
            get { return _model.Load; }
            set { SetBackingField(() => _model.Load = value); }
        }

        public string Name
        {
            get { return _model.UserDetails.UserName; }
            set { SetBackingField(() => _model.UserDetails.UserName = value); }
        }

        public byte[] PictureData
        {
            get { return _model.PictureData; }
            set { SetBackingField(() => _model.PictureData = value); }
        }

        [DependsUpon(nameof(CapacityFactor))]
        public decimal ScaledCapacity
        {
            get { return _model.UserDetails.ScaledCapacity; }
        }

        public UserStatus Status
        {
            get { return Get(() => Status); }
            set { Set(() => Status, value); }
        }

        public string Uid
        {
            get { return _model.UserDetails.Uid; }
            set { SetBackingField(() => _model.UserDetails.Uid = value); }
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
    }
}
using System;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace  Task3_10.Models
{
    public class Mechanic : INotifyPropertyChanged
    {
        // Events
        public event EventHandler<RepairCompletedEventArgs> RepairCompleted;
        public event PropertyChangedEventHandler PropertyChanged;
        
        private string _name;
        private int _skillLevel; // 1-10
        private bool _isBusy;
        private double _x;
        private double _y;
        
        // Properties with notification
        public string Name 
        { 
            get => _name; 
            set { _name = value; OnPropertyChanged(); } 
        }
        
        public int SkillLevel 
        { 
            get => _skillLevel; 
            set { _skillLevel = value; OnPropertyChanged(); } 
        }
        
        public bool IsBusy 
        { 
            get => _isBusy; 
            set { _isBusy = value; OnPropertyChanged(); } 
        }
        
        public double X 
        { 
            get => _x; 
            set { _x = value; OnPropertyChanged(); } 
        }
        
        public double Y 
        { 
            get => _y; 
            set { _y = value; OnPropertyChanged(); } 
        }
        
        public Mechanic()
        {
            IsBusy = false;
        }
        
        public async Task RepairRig(OilRig rig, int fireSeverity)
        {
            if (IsBusy)
                return;
                
            IsBusy = true;
            
            // Calculate repair time based on skill level and fire severity
            int repairTimeSeconds = (int)(10 * fireSeverity / (double)SkillLevel);
            
            // Simulate repair time
            await Task.Delay(repairTimeSeconds * 1000);
            
            // Complete repair
            rig.IsOnFire = false;
            rig.IsOperational = true;
            IsBusy = false;
            
            // Notify repair completed
            OnRepairCompleted(new RepairCompletedEventArgs { RepairTimeSeconds = repairTimeSeconds, Rig = rig });
        }
        
        protected virtual void OnRepairCompleted(RepairCompletedEventArgs e)
        {
            RepairCompleted?.Invoke(this, e);
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class RepairCompletedEventArgs : EventArgs
    {
        public int RepairTimeSeconds { get; set; }
        public OilRig Rig { get; set; }
    }
}
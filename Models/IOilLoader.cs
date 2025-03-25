using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task3_10.Models
{
    public interface IOilLoader
    {
        event EventHandler<LoadingCompletedEventArgs> LoadingCompleted;
        
        string Name { get; set; }
        double Capacity { get; set; }
        double CurrentLoad { get; set; }
        bool IsBusy { get; set; }
        double X { get; set; }
        double Y { get; set; }
        
        Task LoadOil(OilRig rig, double amount);
        Task TransportOil();
    }
    
    public class LoadingCompletedEventArgs : EventArgs
    {
        public double Amount { get; set; }
        public OilRig Rig { get; set; }
    }
}
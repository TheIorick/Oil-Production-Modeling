using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Task3_10.Models
{
    public class Mechanic
    {
        // Событие, когда механик начинает ремонт
        public event EventHandler? RepairStarted;
        
        // Событие, когда механик завершает ремонт
        public event EventHandler? RepairCompleted;
        
        public string Name { get; }
        public int SkillLevel { get; }  // От 1 до 10
        public bool IsBusy { get; private set; }
        
        public Mechanic(string name, int skillLevel)
        {
            if (skillLevel < 1 || skillLevel > 10)
                throw new ArgumentOutOfRangeException(nameof(skillLevel), "Skill level must be between 1 and 10");
                
            Name = name;
            SkillLevel = skillLevel;
            IsBusy = false;
        }

        // Асинхронный метод для ремонта нефтяной вышки
        public async Task RepairRig(OilRig rig, CancellationToken cancellationToken = default)
        {
            if (IsBusy)
                throw new InvalidOperationException("Mechanic is already busy");
                
            if (!rig.IsOnFire && rig.Status != OilRigStatus.Damaged)
                throw new InvalidOperationException("Rig doesn't need repairs");
                
            IsBusy = true;
            
            // Вызываем событие начала ремонта
            RepairStarted?.Invoke(this, EventArgs.Empty);
            
            try
            {
                // Время ремонта зависит от навыка механика (чем выше навык, тем быстрее ремонт)
                int repairTimeSeconds = 20 - SkillLevel;
                await Task.Delay(TimeSpan.FromSeconds(repairTimeSeconds), cancellationToken);
                
                if (!cancellationToken.IsCancellationRequested)
                {
                    // Тушим пожар и восстанавливаем вышку
                    rig.ExtinguishFire();
                    rig.Status = OilRigStatus.Operational;
                }
            }
            finally
            {
                IsBusy = false;
                
                // Вызываем событие завершения ремонта
                RepairCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.ViewModels
{
    public class BodySpeakerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _bodyHeight;
        public string BodyHeight
        {
            get => _bodyHeight;
            set
            {
                _bodyHeight = value;
                OnPropertyChanged(nameof(BodyHeight));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _bodyWidth;
        public string BodyWidth
        {
            get => _bodyWidth;
            set
            {
                _bodyWidth = value;
                OnPropertyChanged(nameof(BodyWidth));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _bodyThickness;
        public string BodyThickness
        {
            get => _bodyThickness;
            set
            {
                _bodyThickness = value;
                OnPropertyChanged(nameof(BodyThickness));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _waterproofType;
        public string WaterproofType
        {
            get => _waterproofType;
            set
            {
                _waterproofType = value;
                OnPropertyChanged(nameof(WaterproofType));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _bodyBackMaterial;
        public string BodyBackMaterial
        {
            get => _bodyBackMaterial;
            set
            {
                _bodyBackMaterial = value;
                OnPropertyChanged(nameof(BodyBackMaterial));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _speakerType;
        public string SpeakerType
        {
            get => _speakerType;
            set
            {
                _speakerType = value;
                OnPropertyChanged(nameof(SpeakerType));
                OnPropertyChanged(nameof(Progress));
            }
        }

        public double Progress
        {
            get
            {
                int filled = 0;

                if (!string.IsNullOrWhiteSpace(BodyHeight)) filled++;
                if (!string.IsNullOrWhiteSpace(BodyWidth)) filled++;
                if (!string.IsNullOrWhiteSpace(BodyThickness)) filled++;
                if (!string.IsNullOrWhiteSpace(WaterproofType)) filled++;
                if (!string.IsNullOrWhiteSpace(SpeakerType)) filled++;

                return filled / 8.0;
            }
        }
    }
}

using BirdClassifier.Helpers;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BirdClassifier.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Main_Page : ContentPage
    {
        public int currcount = 0;
        private ImageSource _SelectedImageSource { get; set; }

        public Command PickPhoto { get; set; }
        public Command TakePhoto { get; set; }

        public ImageSource SelectedImageSource
        {
            get
            {
                return _SelectedImageSource;
            }
            set
            {
                _SelectedImageSource = value;
                SelectedImage.Source = _SelectedImageSource;
            }
        }

        private CognitiveServices Classifier;

        private string _ClassifierResult { get; set; }
        public string ClassifierResult
        {
            get
            {
                return _ClassifierResult;
            }
            set
            {
                _ClassifierResult = value;
                ClassifierResult_Label.Text = _ClassifierResult;
            }
        }

        public Main_Page()
        {
            InitializeComponent();

            PickPhoto = new Command(PickPhoto_Tapped);
            TakePhoto = new Command(TakePhoto_Tapped);

            Classifier = new CognitiveServices();

            BindingContext = this;
        }

        public async void PickPhoto_Tapped()
        {
            currcount = 0;
            ClassifierResult_Label.Text = string.Empty;

            Plugin.Media.Abstractions.MediaFile file = null;

            await CrossMedia.Current.Initialize();

            try
            {
                file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });

                if(file == null)
                {
                    return;
                }

                    SelectedImageSource = ImageSource.FromStream(() =>
                    {
                        try
                        {
                            var stream = file.GetStream();
                            return stream;
                        }
                        catch
                        {
                            return null;
                        }
                    });

                ClassifierResult = await Classifier.GetImageTags(file.GetStream());
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async void TakePhoto_Tapped()
        {
            currcount = 1;
            ClassifierResult_Label.Text = string.Empty;

            Plugin.Media.Abstractions.MediaFile file = null;

            await CrossMedia.Current.Initialize();

            try
            {
                if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No camera found", ":( No camera available.", "Ok");
                    return;
                }

                file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample", 
                    Name = "Xamarin.jpg"
                });

                if(file == null)
                {
                    return;
                }

                SelectedImageSource = ImageSource.FromStream(() =>
                {
                    try
                    {
                        var stream = file.GetStream();
                        return stream;
                    }
                    catch
                    {
                        return null;
                    }
                });

                ClassifierResult = await Classifier.GetImageTags(file.GetStream());
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
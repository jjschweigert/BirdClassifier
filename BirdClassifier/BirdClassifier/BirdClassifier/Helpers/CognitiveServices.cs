using BirdClassifier.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System.Diagnostics;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

namespace BirdClassifier.Helpers
{
    public class CognitiveServices : ICognitiveServices
    {
        private CustomVisionPredictionClient _PredictionClient = null;
        private CustomVisionTrainingClient _TrainingClient = null;
        private Guid _IterationId = Guid.Empty;
        private Guid _ProjectId = Guid.Empty;
        private string _Endpoint = "https://southcentralus.api.cognitive.microsoft.com";
        private string _PredictionKey = "44268715850242d5816df3324c079c3e";
        private string _TrainingKey = "eab48c6ee38740c9bdd6e9e8da1c8542";


        public CustomVisionTrainingClient TrainingClient
        {
            get
            {
                return _TrainingClient;
            }
            set
            {
                _TrainingClient = value;
            }
        }
        public CustomVisionPredictionClient PredictionClient
        {
            get
            {
                return _PredictionClient;
            }
            set
            {
                _PredictionClient = value;
            }
        }
        public Guid ProjectId
        {
            get
            {
                return _ProjectId;
            }
            set
            {
                _ProjectId = value;
            }
        }
        public Guid IterationId
        {
            get
            {
                return _IterationId;
            }
            set
            {
                _IterationId = value;
            }
        }
        public string PredictionKey
        {
            get
            {
                return _PredictionKey;
            }
            set
            {
                _PredictionKey = value;
            }
        }
        public   string TrainingKey
        {
            get
            {
                return _TrainingKey;
            }
            set
            {
                _TrainingKey = value;
            }
        }
        public string Endpoint
        {
            get
            {
                return _Endpoint;
            }
            set
            {
                _Endpoint = value;
            }
        }
        public CognitiveServices()
        {
            _Endpoint = "https://southcentralus.api.cognitive.microsoft.com";

            PredictionClient = new CustomVisionPredictionClient
            {
                ApiKey = PredictionKey,
                Endpoint = Endpoint
            };

            TrainingClient = new CustomVisionTrainingClient
            {
                ApiKey = TrainingKey,
                Endpoint = Endpoint
            };

            ProjectId = GetProject(ProjectNumber: 0).Id;
            IterationId = GetIteration(IterationNumber: 0).Id;
        }
        public Project CurrentProject
        {
            get
            {
                return GetProject(string.Empty, ProjectId);
            }
        }
        public Iteration CurrentIteration
        {
            get
            {
                return GetIteration(string.Empty, IterationId);
            }
        }

        public async Task<string> GetImageTags(Stream imageStream)
        {
            var result = await PredictionClient.PredictImageAsync(ProjectId, imageStream, IterationId);

            string resultStr = string.Empty;

            foreach (PredictionModel pred in result.Predictions)
            {
                if (pred.Probability >= 0.85)
                    resultStr += pred.TagName + " ";
            }

            if(resultStr == string.Empty)
            {
                resultStr = "Unable to classify image.";
            }

            return resultStr;
        }

        public List<Project> GetProjects()
        {
            if (TrainingClient != null)
            {
                List<Project> temp = TrainingClient.GetProjects().ToList();
                return temp;
            }
            else
            {
                return new List<Project>();
            }
        }

        public List<Iteration> GetIterations()
        {
            if (ProjectId != Guid.Empty)
            {
                List<Iteration> temp = TrainingClient.GetIterations(ProjectId).ToList();
                return temp;
            }
            else
            {
                return new List<Iteration>();
            }
        }

        public Project GetProject(string ProjectName = "", Guid ProjectId = new Guid(), int ProjectNumber = -1)
        {
            if (TrainingClient == null)
                return null;

            if(ProjectId != Guid.Empty && string.IsNullOrEmpty(ProjectName))
            {
                return TrainingClient.GetProject(ProjectId);
            }
            else if(ProjectId == Guid.Empty && !string.IsNullOrEmpty(ProjectName))
            {
                return GetProjects().Where(x => x.Name == ProjectName).FirstOrDefault(null);
            }
            else if(ProjectId != Guid.Empty && !string.IsNullOrEmpty(ProjectName))
            {
                var temp = TrainingClient.GetProject(ProjectId);
                if (temp.Name.ToLower() == ProjectName.ToLower())
                {
                    return temp;
                }
            }
            else if(ProjectNumber > -1)
            {
                return GetProjects()[ProjectNumber];
            }

            return null;
        }

        public Iteration GetIteration(string IterationName = "", Guid IterationId = new Guid(), int IterationNumber = -1)
        {
            if (ProjectId == Guid.Empty)
                return null;

            if (IterationId != Guid.Empty && string.IsNullOrEmpty(IterationName))
            {
                return TrainingClient.GetIteration(ProjectId, IterationId);
            }
            else if (IterationId == Guid.Empty && !string.IsNullOrEmpty(IterationName))
            {
                return GetIterations().Where(x => x.Name == IterationName).FirstOrDefault(null);
            }
            else if (IterationId != Guid.Empty && !string.IsNullOrEmpty(IterationName))
            {
                var temp = TrainingClient.GetIteration(ProjectId, IterationId);
                if (temp.Name.ToLower() == IterationName.ToLower())
                {
                    return temp;
                }
            }
            else if(IterationNumber > -1)
            {
                return GetIterations()[IterationNumber];
            }

            return null;
        }
    }
}

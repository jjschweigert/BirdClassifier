using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BirdClassifier.Interfaces
{
    public interface ICognitiveServices
    {
        Guid ProjectId { get; set; }
        Guid IterationId { get; set; }
        string PredictionKey { get; }
        string TrainingKey { get; }
        string Endpoint { get; set; }
        CustomVisionPredictionClient PredictionClient { get; set; }
        CustomVisionTrainingClient TrainingClient { get; set; }
        Project CurrentProject { get; }
        Iteration CurrentIteration { get; }

        Task<string> GetImageTags(Stream imageStream);
        List<Project> GetProjects();
        List<Iteration> GetIterations();
        Project GetProject(string ProjectName = "", Guid ProjectId = new Guid(), int ProjectNumber = -1);
        Iteration GetIteration(string IterationName = "", Guid IterationId = new Guid(), int IterationNumber = -1);
    }
}

using UnityEngine;
using System.Collections;

public class GeneticSettings {
    
    // probability, for each new individual, to mutate 
    public const float mutationProbability = 0.03f;

    // max number of individuals
    public const int maxPopulation = 30;
    // individuals that will be used to generate the next generation
    public const int numberOfBest = 20;
    // individuals that will be reinjected to the next generation
    public const int reinjectedIndividual = 5;

    // modify this parameter to save the statistics of more or less drones
    private const float modOfSavedIndividualForEachGeneration = 1;
    public static bool hasToBeSaved(int id) { return (id % modOfSavedIndividualForEachGeneration == 0); }

    // fitness functions of the individuals
    const float minFitness = -5000;
    const float maxFitness = 5000;
    public static float normalizeFitness(float f) { return droneSettings.normalizeBetween(f, minFitness, maxFitness); }

    // Variable used to avoid the monopolization of the new born from one single parents
    // it is setted high because it is not a real problem
    public const int maxNumberOfSons = 30;

    // life of a drone, it'll be increased until it reaches maxTime
    public static float timelimit = 20;
    public static float maxTime = 240;

}

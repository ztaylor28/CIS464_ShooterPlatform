using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public static class Particles
{
    private static Transform[] particles = null;

    public static void PlayParticle(string particleName, Transform root)
    {
        if(particles == null)
            particles = Resources.LoadAll<Transform>("Prefabs/Particles");

        foreach(Transform particle in particles)
        {
            if (particle.name == particleName) // found it!
            {
                Transform partObj = GameObject.Instantiate(particle);
                partObj.position = root.position;
                break;
            }
        }
    }

    public static void PlayParticle(string particleName, Color[] colors, Transform root)
    {
        if(particles == null)
            particles = Resources.LoadAll<Transform>("Prefabs/Particles");

        foreach(Transform particle in particles)
        {
            if (particle.name == particleName) // found it!
            {
                Transform partObj = GameObject.Instantiate(particle);
                ParticleSystem.MainModule particleSystem = partObj.GetComponent<ParticleSystem>().main;

                Gradient gradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[colors.Length];

                for (int i = 0; i < colors.Length; i++)
                {
                    colorKeys[i].color = colors[i];
                    colorKeys[i].time = (float)(i + 1) / (colors.Length);
                }

                gradient.colorKeys = colorKeys;
                gradient.mode = GradientMode.Fixed; //we want it to be fixed as it's random colors

                MinMaxGradient startColor = new MinMaxGradient(gradient);
                startColor.mode = ParticleSystemGradientMode.RandomColor;
                particleSystem.startColor = startColor;

                partObj.position = root.position;
                break;
            }
        }
    }
}

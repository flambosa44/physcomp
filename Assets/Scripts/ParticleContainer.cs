using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleContainer : MonoBehaviour
{
    private ParticleSystem particleSys;
    private Collider container;
    private ParticleSystem.Particle[] particles;
    private int numParticles;
    // Start is called before the first frame update
    void Start()
    {
        particleSys = this.gameObject.GetComponent<ParticleSystem>();
        container = this.gameObject.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        ParticleSystem.Particle currentParticle;
        particles = new ParticleSystem.Particle[particleSys.particleCount];
        numParticles = particleSys.GetParticles(particles);
        for(int i = 0; i < numParticles; i++)
        {
            currentParticle = particles[i];
            if(!container.bounds.Contains(currentParticle.position))
            {
                currentParticle.remainingLifetime = -1.0f;
                particles[i] = currentParticle;
            }

        }
        particleSys.SetParticles(particles);
    }
}

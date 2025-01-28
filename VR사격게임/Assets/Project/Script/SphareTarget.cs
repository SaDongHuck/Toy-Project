using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SphareTarget : MonoBehaviour
{
    public SphareSpawner spawner;
    public int scoreValue = 10;   // 구체를 맞췄을 때 얻는 점수
    public ParticleSystem HitParticle;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile")) // 투사체와 충돌
        {
            LaunchController launchController = FindObjectOfType<LaunchController>();
            if (launchController != null)
            {
                launchController.AddScore(10); // 점수 추가
            }
            if(HitParticle != null && collision.contacts.Length > 0)
            {
                Vector3 contactPoint = collision.contacts[0].point;
                ParticleSystem particleInstance = Instantiate(HitParticle, contactPoint, Quaternion.identity);
                var main = particleInstance.main; // main 속성 참조 저장
                Destroy(particleInstance.gameObject, main.duration);
            }
            spawner.SphereHit(gameObject); // 구체 제거
          
        }
    }

    
}

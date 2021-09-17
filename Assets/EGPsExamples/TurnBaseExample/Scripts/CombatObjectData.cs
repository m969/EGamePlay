using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay.Combat;

public class CombatObjectData : MonoBehaviour
{
    public AnimationComponent AnimationComponent;
    public Text DamageText;
    public Text CureText;
    public Image HealthBarImage;
    public Transform CanvasTrm;
    public Transform StatusSlotsTrm;
    public GameObject StatusIconPrefab;
    public GameObject vertigoParticle;
    public GameObject weakParticle;
}

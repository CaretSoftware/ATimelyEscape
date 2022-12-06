using CallbackSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace CallbackSystem
{
    [RequireComponent(typeof(CubePush))]
    [RequireComponent(typeof(TimeTravelObject))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CubeCharge : MonoBehaviour
    {
        private static readonly int chargeReductionAfterTimeJump = 1;

        [Tooltip("Charge reducese with 1 every forward jump in time. At 0 cube can no longer be puched")]
        public int charge = 0;
        [SerializeField] private float timeToCharge;
        [HideInInspector] public CubeCharge pastCubeCharge;


        private CubePush cubePush;
        private ChargeChangedEvent chargeEvent;
        private TimeTravelObject timeTravelObject;
        private MeshRenderer meshRenderer;
        private PingPong pingPong;

        private IconBehaviour iconBehaviour;

        private void Start()
        {
            iconBehaviour = GetComponentInChildren<IconBehaviour>();

            cubePush = GetComponent<CubePush>();
            timeTravelObject = GetComponent<TimeTravelObject>();
            meshRenderer = GetComponent<MeshRenderer>();
            pingPong = GetComponent<PingPong>();
            if (charge > 0)
            {
                cubePush.SetPushable(true);
                iconBehaviour.IsCharged(true);

            }
            else
            {
                cubePush.SetPushable(false);
                iconBehaviour.IsCharged(false);
            }

            chargeEvent = new(timeTravelObject);

            if (timeTravelObject.pastSelf != null && timeTravelObject.pastSelf.gameObject.GetComponent<CubeCharge>() != null)
            {
                pastCubeCharge = timeTravelObject.pastSelf.gameObject.GetComponent<CubeCharge>();
            }
            ChargeChangedEvent.AddListener<ChargeChangedEvent>(PastChargeChange);
            TimePeriodChanged.AddListener<TimePeriodChanged>(changedTime);
        }

        public void Charging(int charge, Object origin)
        {
            if (this.charge < charge)
            {
                this.charge = charge;
                if (charge > 0)
                {
                    cubePush.SetPushable(true);
                    iconBehaviour.IsCharged(true);
                    StartCoroutine(SetMaterial(true));
                }
                else
                {
                    StartCoroutine(SetMaterial(false));
                    iconBehaviour.IsCharged(false);
                }
                if (!origin.GetType().Equals(GetType()))
                {
                    chargeEvent.Invoke();
                }
            }
        }

        private void PastChargeChange(ChargeChangedEvent chargeChangedEvent)
        {
            if (timeTravelObject.pastSelf.pastSelf != null & timeTravelObject.pastSelf.pastSelf.Equals(chargeChangedEvent.changedObject))
            {
                Charging(pastCubeCharge.pastCubeCharge.charge - (2 * chargeReductionAfterTimeJump), this);
            }
            if (timeTravelObject.pastSelf != null & timeTravelObject.pastSelf.Equals(chargeChangedEvent.changedObject))
            {
                Charging(pastCubeCharge.charge - chargeReductionAfterTimeJump, this);
            }
        }

        private IEnumerator SetMaterial(bool on)
        {
            float timer = 0f;
            while (timer < 0)
            {
                if (on)
                {
                    timer += Time.deltaTime / timeToCharge;
                    pingPong.SetPower(Mathf.Lerp(0, 1, timer));
                    yield return null;
                }
                else
                {
                    timer += Time.deltaTime / timeToCharge;
                    pingPong.SetPower(Mathf.Lerp(1, 0, timer));
                    yield return null;
                }
            }
        }
        private void changedTime(TimePeriodChanged timePeriodChanged)
        {
            if (charge > 0)
            {
                pingPong.SetPower(1);
            }
            else
            {
                pingPong.SetPower(0);
            }
        }
    }
}

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
        /*[HideInInspector]*/ public CubeCharge pastCubeCharge;


        private CubePush cubePush;
        private ChargeChangedEvent chargeEvent;
        [SerializeField] private TimeTravelObject timeTravelObject;
        private MeshRenderer meshRenderer;
        private PingPong pingPong;

        private IconBehaviour iconBehaviour;

        private void Start()
        {
            
            //iconBehaviour = GetComponentInChildren<IconBehaviour>();

            cubePush = GetComponent<CubePush>();
            timeTravelObject = GetComponent<TimeTravelObject>();
            meshRenderer = GetComponent<MeshRenderer>();
            pingPong = GetComponent<PingPong>();
            // if (pingPong == null)
            // {
            //     Debug.Log("¯\\_(ツ)_/¯ Patrik says the PingPong.cs Script is missing from this cube. He's not sure if it should be here or not, so he adds it anyway.", this);
            //     pingPong = gameObject.AddComponent<PingPong>(); // TODO SOMETIMES MAYBE GOOD, SOMETIMES MAYBE SHIT!
            // }
            if (charge > 0)
            {
                cubePush.SetPushable(true);
                //iconBehaviour.IsCharged(true);
                pingPong.SetPower(1f);

            }
            else
            {
                cubePush.SetPushable(false);
                //iconBehaviour.IsCharged(false);
                pingPong.SetPower(0f);
                
            }

            chargeEvent = new(timeTravelObject);

            StartCoroutine(SetPast());
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
                    StartCoroutine(SetMaterial(true));
                    if (cubePush != null)
                        cubePush.SetPushable(true);
                }
                else
                {
                    StartCoroutine(SetMaterial(false));
                }
                if (!origin.GetType().Equals(GetType()))
                {
                    chargeEvent?.Invoke();
                }
            }
        }

        private void PastChargeChange(ChargeChangedEvent chargeChangedEvent)
        {
            if (timeTravelObject.pastSelf != null && timeTravelObject.pastSelf.pastSelf != null && timeTravelObject.pastSelf.pastSelf.Equals(chargeChangedEvent.changedObject))
            {
                    Charging(pastCubeCharge.pastCubeCharge.charge - (2 * chargeReductionAfterTimeJump), this);
            }
            if (timeTravelObject.pastSelf != null && timeTravelObject.pastSelf.Equals(chargeChangedEvent.changedObject))
            {
                
                    Charging(pastCubeCharge.charge - chargeReductionAfterTimeJump, this);
            }
        }

        private void PastChargeChange()
        {
            /*if (timeTravelObject.pastSelf != null && timeTravelObject.pastSelf.pastSelf != null)
            {
//                Charging(pastCubeCharge.pastCubeCharge.charge - (2 * chargeReductionAfterTimeJump), this);
            }
            if (timeTravelObject.pastSelf != null)
            {
                Charging(pastCubeCharge.charge - chargeReductionAfterTimeJump, this);
            }*/
        }

        private IEnumerator SetMaterial(bool on)
        {
            float timer = 0f;
            while (timer < 1f)
            {
                if (on)
                {
                    timer += Time.deltaTime / timeToCharge;
                    if (pingPong != null)
                        pingPong.SetPower(Mathf.Lerp(0f, 1f, timer));
                    yield return null;
                }
                else
                {
                    timer += Time.deltaTime / timeToCharge;
                    if (pingPong != null)
                        pingPong.SetPower(Mathf.Lerp(1f, 0f, timer));
                    yield return null;
                }
            }
            //if (charge > 0)
            //{
            //    iconBehaviour.IsCharged(true);

            //}
            //else
            //{
            //    iconBehaviour.IsCharged(false);
            //}
        }
        private void changedTime(TimePeriodChanged timePeriodChanged)
        {
            if (charge > 0f)
            {
                pingPong.SetPower(1f);
                //iconBehaviour.IsCharged(true);
            }
            else
            {
                pingPong.SetPower(0f);
                //iconBehaviour.IsCharged(false);
            }
            PastChargeChange();
        }
        private IEnumerator SetPast()
        {
            yield return null;
            if (timeTravelObject.pastSelf != null && timeTravelObject.pastSelf.gameObject.GetComponent<CubeCharge>() != null)
            {
                pastCubeCharge = timeTravelObject.pastSelf.gameObject.GetComponent<CubeCharge>();
            }
        }

    }
}

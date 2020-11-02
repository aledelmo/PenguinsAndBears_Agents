using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
namespace Penguin.Scripts
{
    public class PenguinAgent : Agent
    {
        [Tooltip("How fast the agent moves forward")]
        public float moveSpeed = 5f;

        [Tooltip("How fast the agent turns")]
        public float turnSpeed = 180f;

        [Tooltip("Prefab of the heart that appears when the baby is fed")]
        public GameObject heartPrefab;

        [Tooltip("Prefab of the regurgitated fish that appears when the baby is fed")]
        public GameObject regurgitatedFishPrefab;
    
        private PenguinArea penguinArea;
        private new Rigidbody rigidbody;
        private GameObject baby;
        private bool isFull; // If true, penguin has a full stomach
        private float feedRadius;
    
        /// <summary>
        /// Initial setup, called when the agent is enabled
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            penguinArea = GetComponentInParent<PenguinArea>();
            baby = penguinArea.penguinBaby;
            rigidbody = GetComponent<Rigidbody>();
        }
    
        /// <summary>
        /// Perform actions based on a vector of numbers
        /// </summary>
        /// <param name="vectorAction">The list of actions to take</param>
        public override void OnActionReceived(float[] vectorAction)
        {
            // Convert the first action to forward movement
            var forwardAmount = vectorAction[0];

            // Convert the second action to turning left or right
            float turnAmount;
            switch (vectorAction[1])
            {
                case 1f:
                    turnAmount = -1f;
                    break;
                case 2f:
                    turnAmount = 1f;
                    break;
                default:
                    turnAmount = 0f;
                    break;
            }

            // Apply movement
            var transform1 = transform;
            rigidbody.MovePosition(transform1.position + Time.fixedDeltaTime * forwardAmount * moveSpeed * transform1.forward);
            transform.Rotate(Time.fixedDeltaTime * turnAmount * turnSpeed * transform.up);

            // Apply a tiny negative reward every step to encourage action
            if (MaxStep > 0) AddReward(-1f / MaxStep);
        }
    
        /// <summary>
        /// Read inputs from the keyboard and convert them to a list of actions.
        /// This is called only when the player wants to control the agent and has set
        /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
        /// </summary>
        /// <returns>A vectorAction array of floats that will be passed into </returns>
        public override void Heuristic(float[] actionsOut)
        {
            var forwardAction = 0f;
            var turnAction = 0f;
            if (Input.GetKey(KeyCode.W))
            {
                // move forward
                forwardAction = 1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                // turn left
                turnAction = 1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                // turn right
                turnAction = 2f;
            }

            // Put the actions into an array and return
            actionsOut[0] = forwardAction;
            actionsOut[1] = turnAction;
        }
    
        /// <summary>
        /// Reset the agent and area
        /// </summary>
        public override void OnEpisodeBegin()
        {
            isFull = false;
            penguinArea.ResetArea();
            feedRadius = Academy.Instance.EnvironmentParameters.GetWithDefault("feed_radius", 0f);
        }
    
        /// <summary>
        /// Collect all non-Raycast observations
        /// </summary>
        public override void CollectObservations(VectorSensor sensor)
        {
            // Whether the penguin has eaten a fish (1 float = 1 value)
            sensor.AddObservation(isFull);

            // Distance to the baby (1 float = 1 value)
            var position = baby.transform.position;
            var position1 = transform.position;
            sensor.AddObservation(Vector3.Distance(position, position1));

            // Direction to baby (1 Vector3 = 3 values)
            sensor.AddObservation((position - position1).normalized);

            // Direction penguin is facing (1 Vector3 = 3 values)
            sensor.AddObservation(transform.forward);

            // 1 + 1 + 3 + 3 = 8 total values
        }
    
        private void FixedUpdate()
        {
            // Request a decision every 5 steps. RequestDecision() automatically calls RequestAction(),
            // but for the steps in between, we need to call it explicitly to take action using the results
            // of the previous decision
            if (StepCount % 5 == 0)
            {
                RequestDecision();
            }
            else
            {
                RequestAction();
            }

            // Test if the agent is close enough to to feed the baby
            if (Vector3.Distance(transform.position, baby.transform.position) < feedRadius)
            {
                // Close enough, try to feed the baby
                RegurgitateFish();
            }
        }
    
        /// <summary>
        /// When the agent collides with something, take action
        /// </summary>
        /// <param name="collision">The collision info</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("fish"))
            {
                // Try to eat the fish
                EatFish(collision.gameObject);
            }
            else if (collision.transform.CompareTag("baby"))
            {
                // Try to feed the baby
                RegurgitateFish();
            }
            else if (collision.transform.CompareTag("bear"))
            {
                // Poor Penguin
                EatPenguin();
            }
            
        }
    
        /// <summary>
        /// Check if agent is full, if not, eat the fish and get a reward
        /// </summary>
        /// <param name="fishObject">The fish to eat</param>
        private void EatFish(GameObject fishObject)
        {
            if (isFull) return; // Can't eat another fish while full
            isFull = true;

            penguinArea.RemoveSpecificFish(fishObject);

            AddReward(1f);
        }
    
        /// <summary>
        /// Check if agent is full, if yes, feed the baby
        /// </summary>
        private void RegurgitateFish()
        {
            if (!isFull) return; // Nothing to regurgitate
            isFull = false;

            // Spawn regurgitated fish
            var parent = transform.parent;
            GameObject regurgitatedFish = Instantiate(regurgitatedFishPrefab, parent, true);
            var position = baby.transform.position;
            regurgitatedFish.transform.position = position;
            Destroy(regurgitatedFish, 4f);

            // Spawn heart
            var heart = Instantiate(heartPrefab, parent, true);
            heart.transform.position = position + Vector3.up;
            Destroy(heart, 4f);

            AddReward(1f);

            if (penguinArea.FishRemaining <= 0)
            {
                EndEpisode();
            }
        }

        private void EatPenguin()
        {
            AddReward(-4f);
            EndEpisode();
        }
    }
}
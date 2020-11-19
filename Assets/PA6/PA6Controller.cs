using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PA6Controller : MonoBehaviour {

    public int progress = 0;

    public int[] scores = new int[] { 0, 0, 0, 0, 0 };
    public Text[] scoreLabels;
    public Text titleLabel;
    public Text messageLabel;

    public Targetable bullseye;
    public Spaceship ship;

    private void Start() {
        StartCoroutine(Run());
    }

    IEnumerator Run() {
        BaseAgent agent = ship.GetComponent<BaseAgent>();
        TargetingSystem targetingSystem = ship.GetSystem<TargetingSystem>();

        foreach (Text scoreLabel in scoreLabels)
            scoreLabel.text = "";
        titleLabel.text = "";
        messageLabel.text = "";
        bullseye.gameObject.SetActive(false);
        foreach (PA_6_2_EnemyAgent enemy in pa2_Enemies)
            enemy.gameObject.SetActive(false);
        asteroidField.gameObject.SetActive(false);

        // 6.1: Move to position and stop
        if (progress == 0) {
            scoreLabels[0].text = $"6.1: {scores[0]}/10";
            titleLabel.text = $"6.1: Stop on Bullseye";
            for (int i = 0; i < 5; i++) {
                float timer = 0;
                Vector3 targetPosition = pa1_MaxRange * Random.insideUnitSphere;
                targetPosition.y = 0;
                bullseye.transform.position = targetPosition;
                bullseye.gameObject.SetActive(true);
                ship.Reset();
                agent.ship.Rigidbody.velocity = Vector3.zero;
                while (!PA6_1_Completed() && timer < pa1_MaxTimePerTest) {

                    agent.Run_6_1(bullseye);

                    messageLabel.text = $"Time Remaining: {pa1_MaxTimePerTest - timer:F1}";
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                }

                int score = PA6_1_Completed() ? 2 : 0;
                Debug.Log($"{ship.Owner.Name} scored {score} on PA6.1.{i + 1}");
                scores[0] += score;
                scoreLabels[0].text = $"6.1: {scores[0]}/10";
            }
            Debug.Log($"{ship.Owner.Name} scored {scores[0]} on PA6.1");
            bullseye.gameObject.SetActive(false);
            progress++;
        }

        // 6.2: Evade fire
        if (progress == 1) {
            scores[1] = 10; // start with 10 points, lose 1 point per hit
            scoreLabels[1].text = $"6.2: {scores[1]}/10";
            titleLabel.text = $"6.2: Evade Enemy Fire";
            messageLabel.text = "";
            ship.Reset();
            ship.transform.position = Vector3.zero;
            ship.Destructible.OnDamaged += (Destructible d, float amount) =>
            {
                scores[1] = Mathf.Max(0, scores[1] - 1);
                scoreLabels[1].text = $"6.2: {scores[1]}/10";
            };
            
            foreach (PA_6_2_EnemyAgent enemy in pa2_Enemies)
                enemy.gameObject.SetActive(true);

            yield return new WaitForSeconds(5f);
            
            foreach (PA_6_2_EnemyAgent enemy in pa2_Enemies)
                enemy.canFire = true;
            float pa2_timer = 30;
            while (pa2_timer > 0) {
                messageLabel.text = $"Time Remaining: {pa2_timer:F1}";
                agent.Run_6_2();
                yield return new WaitForEndOfFrame();
                pa2_timer -= Time.deltaTime;
            }
            ship.Destructible.OnDamaged = null;
            foreach (PA_6_2_EnemyAgent enemy in pa2_Enemies)
                enemy.gameObject.SetActive(false);
            progress++;
        }

        // 6.3: Target & fire
        if (progress == 2) {
            scoreLabels[2].text = $"6.3: {scores[2]}/10";
            titleLabel.text = $"6.3: Shoot Targets";
            messageLabel.text = "";
            Targetable target = pa3_target;
            target.gameObject.SetActive(true);
            for (int i = 0; i < 10; i++) {
                float timer = 0;
                bool targetHit = false;

                target.GetComponent<Spaceship>().Reset();
                Vector3 targetPos = Random.insideUnitSphere * 100f;
                targetPos.y = 0;
                target.transform.position = targetPos;
                target.Destructible.OnDamaged = null;
                target.Destructible.OnDamaged += (d, amount) => { targetHit = true; };

                ship.Reset();
                ship.transform.position = Vector3.zero;
                ship.transform.rotation = Quaternion.identity;
                
                while (!targetHit && timer < 10) {
                    messageLabel.text = $"Time Remaining: {10-timer:F1}";
                    agent.Run_6_3(target);

                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                }

                if (targetHit) {
                    scores[2]++;
                    scoreLabels[2].text = $"6.3: {scores[2]}/10";
                }
                ship.Reset();

                yield return new WaitForSeconds(3f);
            }

            target.gameObject.SetActive(false);
            progress++;
        }

        // 6.4: Navigate w/o collision
        if (progress == 3) {
            scoreLabels[3].text = $"6.4: {scores[3]}/10";
            titleLabel.text = $"6.4: Avoid Collisions";
            messageLabel.text = "";
            float maxTime = pa4_MaxTimePerTest;
            asteroidField.gameObject.SetActive(true);

            for (int i = 0; i < 5; i++) {
                float timer = 0;
                bool targetReached = false;
                bool hitAnyAsteroids = false;

                ship.OnCollision = (s, collision) =>
                {
                    hitAnyAsteroids = true;
                };

                ship.Reset();
                ship.transform.position = Vector3.zero;

                Vector3 targetPos;
                float targetSafeRadius = 40f;
                while (true) {
                    targetPos = Random.insideUnitSphere * 300f;
                    targetPos.y = 0;
                    Collider[] overlaps = Physics.OverlapSphere(targetPos, targetSafeRadius);
                    if (overlaps.Length == 0) {
                        break;
                    }
                }
                bullseye.transform.position = targetPos;
                bullseye.gameObject.SetActive(true);

                while (timer < maxTime && !targetReached) {
                    messageLabel.text = $"Time Remaining: {maxTime - timer:F1}";

                    agent.Run_6_4(bullseye);

                    if (!targetReached) {
                        Vector3 delta = ship.transform.position - bullseye.Position;
                        float dist = delta.magnitude;
                        targetReached = dist <= 5f;
                    }

                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                }

                if (targetReached) {
                    scores[3] += hitAnyAsteroids ? 1 : 2;
                    scoreLabels[3].text = $"6.4: {scores[3]}/10";
                }
            }

            progress++;
            bullseye.gameObject.SetActive(false);
            ship.Reset();
            ship.OnCollision = null;
        }

        // 6.5: Class Challenge
        if(progress == 4) {
            scoreLabels[4].text = $"6.5: In Class Challenge";
            progress++;
        }

        titleLabel.text = "Grading Run Complete!";
        int totalScore = 0;
        foreach (int score in scores)
            totalScore += score;
        messageLabel.text = $"Total Score: {totalScore}/40";
        
        yield break;
    }

    public float pa1_MaxRange = 100f;
    public float pa1_MaxTimePerTest = 5f;
    public float pa1_DistanceTolerance = 0.5f;
    public float pa1_SpeedTolerance = 0.01f;
    bool PA6_1_Completed() {
        Vector3 delta = ship.transform.position - bullseye.transform.position;
        bool completed = delta.magnitude <= pa1_DistanceTolerance && ship.Rigidbody.velocity.magnitude <= pa1_SpeedTolerance;
        return completed;
    }

    public PA_6_2_EnemyAgent[] pa2_Enemies;
    public Targetable pa3_target;

    public float pa4_MaxTimePerTest = 20f;

    public GameObject asteroidField;
}

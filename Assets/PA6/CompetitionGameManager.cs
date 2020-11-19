using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompetitionGameManager : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerSetupInfo {
        public string playerName;
        public string agentClass;
    }
    [SerializeField] private List<PlayerSetupInfo> setupInfos = null;

    [SerializeField] private List<Player> players = null;
    [SerializeField] private List<Spaceship> shipPrefabs = null;

    [SerializeField] private List<Text> playerScoreRows = null;
    [SerializeField] private Text playerScoreRowPrefab = null;
    [SerializeField] private RectTransform playerScoreRowParent = null;
    [SerializeField] private Text titleLabel = null;
    [SerializeField] private Text messageLabel = null;

    [SerializeField] private float timePerRound = 3 * 60;
    [SerializeField] private float roundTimer = 3 * 60;
    [SerializeField] private int maxRounds = 5;
    [SerializeField] private int curRound = -1;

    public List<Spaceship> Ships => players.ConvertAll<Spaceship>(p => p.Ship);

    private void Start() {
        Setup();
    }

    Player CreatePlayer(PlayerSetupInfo info) {
        System.Type agentType = GetType().Assembly.GetType(info.agentClass);
        if (agentType == null) {
            Debug.LogError($"Unable to create agent \"{info.agentClass}\" for player {info.playerName}.");
            return null;
        }

        Spaceship shipPrefab = shipPrefabs[Random.Range(0, shipPrefabs.Count)];
        Spaceship ship = Instantiate(shipPrefab);
        Player player = ship.gameObject.AddComponent<Player>();
        
        BaseAgent agent = ship.gameObject.AddComponent(agentType) as BaseAgent;
        player.Name = info.playerName;
        players.Add(player);
        ship.Owner = player;
        player.Ship = ship;
        ship.gameObject.name = player.Name;
        ship.Reset();

        Text shipLabel = ship.GetComponentInChildren<Text>();
        shipLabel.text = player.Name;

        Text scoreRow = Instantiate(playerScoreRowPrefab, playerScoreRowParent);
        playerScoreRows.Add(scoreRow);

        respawnTimers.Add(new RespawnTimer() { player = player, time = 0f });

        return player;
    }

    void Setup() {
        foreach (PlayerSetupInfo info in setupInfos)
            CreatePlayer(info);

        foreach (Player player in players) {
            Cannon cannon = player.Ship.GetSystem<Cannon>();
            if(cannon != null) {
                cannon.OnFire += OnCannonFired;
            }
            player.Ship.Destructible.OnDied += OnShipDestroyed;
        }
    }

    public int PointsPerFire = -1;
    public int PointsPerDeath = -100;
    public int PointsPerHit = 10;
    public int PointsPerKill = 100;
    
    void OnCannonFired(Cannon cannon, Projectile projectile) {
        cannon.Ship.Owner.Score += PointsPerFire;
        projectile.OnImpact += OnProjectileImpact;

    }

    void OnProjectileImpact(Projectile projectile, Destructible target) {
        Player firer = projectile.launcher.Ship.Owner;
        if(firer != null) {
            firer.Score += PointsPerHit;
            Debug.Log($"{firer.name}'s projectile hit {target.name}.");
        }
    }

    void OnShipDamaged(Spaceship ship, float amount) {

    }

    private void Update() {
        roundTimer -= Time.deltaTime;
        if(curRound < maxRounds && roundTimer <= 0) {
            curRound++;
            if(curRound >= maxRounds) {
                OnGameOver();
            } else {
                roundTimer = timePerRound;
            }
        }
        UpdateAgents();
        EnforceBoundary();
        Respawn();
        UpdateUi();
    }

    void OnGameOver() {
        titleLabel.text = "Game Over";
        titleLabel.gameObject.SetActive(true);
    }

    void UpdateAgents() {
        foreach(Player player in players) {
            try {
                BaseAgent agent = player.GetComponent<BaseAgent>();
                if (agent != null)
                    agent.Run_6_5();
            }
            catch(System.Exception ex) {
                Debug.LogException(ex);
                player.Score -= 100;
            }
        }
    }

    void UpdateUi() {
        messageLabel.text = $"Round {curRound+1}: {roundTimer:F0}";

        // TODO: sort players by score
        for(int i = 0; i < playerScoreRows.Count; i++) {
            Player player = players[i];
            Text row = playerScoreRows[i];
            Spaceship ship = player.Ship;
            int hp = Mathf.CeilToInt(ship.Destructible.Health);
            int maxHp = Mathf.CeilToInt(ship.Destructible.MaxHealth);
            row.text = $"{player.Name}: {player.Score} {hp}/{maxHp}";
            row.gameObject.SetActive(true);
        }
    }

    [SerializeField] private float maxDistance = 500;

    void EnforceBoundary() {
        Vector3 origin = Vector3.zero;
        foreach(Spaceship ship in Ships) {
            if (ship.Destructible.IsDead)
                continue;
            Vector3 delta = ship.transform.position - origin;
            float dist = delta.magnitude;
            if(dist > maxDistance) {
                ship.Destructible.DoDamage(ship.Destructible.Health);
            }
        }
    }

    [System.Serializable]
    private class RespawnTimer {
        public Player player;
        public float time;
    }

    [SerializeField] private List<RespawnTimer> respawnTimers = new List<RespawnTimer>();
    
    void OnShipDestroyed(Destructible destructible) {
        Spaceship ship = destructible.GetComponent<Spaceship>();
        Player player = ship.Owner;
        player.Deaths++;
        player.Score += PointsPerDeath;
    }

    void Respawn() {
        foreach (Player player in players) {
            if (player.Ship == null || player.Ship.Destructible.IsDead) {
                RespawnTimer timer = respawnTimers.Find(t => t.player == player);
                if (timer == null) {
                    // TODO: calculate respawn time based on # of deaths
                    timer = new RespawnTimer() { player = player, time = 3f };
                    respawnTimers.Add(timer);
                }
            }
        }

        for(int i = respawnTimers.Count-1; i >= 0; i--) {
            RespawnTimer timer = respawnTimers[i];
            timer.time -= Time.deltaTime;
            if(timer.time <= 0) {
                respawnTimers.RemoveAt(i);
                Respawn(timer.player);
            }
        }
    }

    void Respawn(Player player) {
        int tries = 0;
        Vector3 position = Vector3.zero;
        while (tries < 10) {

            tries++;

            position = Random.insideUnitSphere * 300f;
            position.y = 0;

            Collider[] colliders = Physics.OverlapSphere(position, 40);
            if (colliders.Length == 0)
                break;
        }
        player.Ship.transform.position = position;
        player.Ship.Reset();
        player.Ship.gameObject.SetActive(true);
    }
}

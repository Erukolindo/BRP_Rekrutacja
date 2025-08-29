using UnityEngine;

public class SoulEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject InteractionPanelObject;
    [SerializeField] private GameObject ActionsPanelObject;
    [SerializeField] private SpriteRenderer EnemySpriteRenderer;
    [SerializeField] private int scoreReward = 10;
    [SerializeField] private bool meleeWeakness = false;
    [SerializeField] private bool rangedWeakness = false;

    private SpawnPoint _enemyPosition;

    public void SetupEnemy(Sprite sprite, SpawnPoint spawnPoint)
    {
        EnemySpriteRenderer.sprite = sprite;
        meleeWeakness = Random.Range(0, 2) == 0;
        rangedWeakness = !meleeWeakness;
        _enemyPosition = spawnPoint;
        gameObject.SetActive(true);
    }

    public SpawnPoint GetEnemyPosition()
    {
        return _enemyPosition;
    }

    public GameObject GetEnemyObject()
    {
        return this.gameObject;
    }

    private void ActiveCombatWithEnemy()
    {
        ActiveInteractionPanel(false);
        ActiveActionPanel(true);
    }

    private void ActiveInteractionPanel(bool active)
    {
        InteractionPanelObject.SetActive(active);
    }

    private void ActiveActionPanel(bool active)
    {
        ActionsPanelObject.SetActive(active);
    }

    private void UseBow()
    {
        // USE BOW
        GameEvents.EnemyKilled?.Invoke(this);
        GameControlller.Instance.UpdateScore(Mathf.RoundToInt(scoreReward * (rangedWeakness ? 1.5f : 1)));
    }

    private void UseSword()
    {
        GameEvents.EnemyKilled?.Invoke(this);
        GameControlller.Instance.UpdateScore(Mathf.RoundToInt(scoreReward * (meleeWeakness ? 1.5f : 1)));
        // USE SWORD
    }

    #region OnClicks

    public void Combat_OnClick()
    {
        ActiveCombatWithEnemy();
    }

    public void Bow_OnClick()
    {
        UseBow();
    }

    public void Sword_OnClick()
    {
        UseSword();
    }

    #endregion
}


public interface IEnemy
{
    SpawnPoint GetEnemyPosition();
    GameObject GetEnemyObject();
}

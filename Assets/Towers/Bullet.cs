using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// This class represents the model of a single tower Bullet.
/// </summary>
public class Bullet : MonoBehaviour
{
	/// <summary>
	/// The critter being targeted.
	/// </summary>
	public Critter Target = null;
	/// <summary>
	/// Speed of bullet.
	/// </summary>
	public float speed = 2f;
	/// <summary>
	/// The bullet detection radius.
	/// </summary>
	public float bulletDetectionRadius = 0.01f;
	/// <summary>
	/// Damage inflicted.
	/// </summary>
	public float damage = 20f;
	/// <summary>
	/// The splash damage modifier.
	/// </summary>
	public float splashDamageModifier = 0.25f;
	/// <summary>
	/// The splash damage radius.
	/// </summary>
	public float splashDamageRadius = 0.3f;
	/// <summary>
	/// Used to check whether splash damage is applied or not.
	/// </summary>
	public bool isSplashDamage = false;
	/// <summary>
	/// Modifies critter speed.
	/// </summary>
	public float slowAmount = 0.25f;
	/// <summary>
	/// Used to check whether slow effect is applied or not.
	/// </summary>
	public bool isSlow = false;

	/// Used to check whether the target critter has been shot.
	private bool shotTaken = false;

	/// <summary>
	/// The Rigidbody component representing the bullet.
	/// </summary>
	[SerializeField] Rigidbody rigidBody = null;
	/// <summary>
	/// Container containing all Critter-related info.
	/// </summary>
	[SerializeField] GameObject critterContainer = null;

	/// <summary>
	/// Awake this instance and assigns the CritterContainer to  <see cref="critterContainer"/>.
	/// </summary>
	void Awake()
	{
		critterContainer = GameManager.Instance.WaveManager.CritterContainer;
	}

	/// <summary>
	/// Fixes the update framerate.
	/// 
	/// Destroys this bullet if no target exists or if a shot had been taken.
	/// Updates the bullet trajectory to follow the critter and calls <see cref="InitialShotTriggered()"/>
	/// to register a shot if the trajectory's magnitude is less than the <see cref="bulletDetectionRadius"/>;
	/// otherwise, the trajectory vector is normalized and multiplied by the bullet's speed. 
	/// 
	/// This is to simplify the calculation.
	/// </summary>
	void FixedUpdate ()
	{
		if(Target == null || shotTaken)
		{
			if(gameObject != null)
				Destroy(gameObject);
			return;
		}
		//Changing the velocity of bullet to follow critter
		Vector3 velocity = Target.transform.position - transform.position;
		if(velocity.magnitude < bulletDetectionRadius)
			InitialShotTriggered();
		else
			rigidBody.velocity = velocity.normalized*speed;
	}

	/// <summary>
	/// Triggers the initial shot by calling <see cref="ShotTriggered(Critter critter, float damage, float slowAmount)"/>.
	/// 
	/// More shots are triggered if there are other critters nearby the target within a region 
	/// centered at the target with a radius determined by <see cref="splashDamageRadius"/> if 
	/// splash damage applies. <see cref="shotTaken"/> is set to True.
	/// </summary>
	void InitialShotTriggered()
	{
		ShotTriggered(Target,damage,isSlow?slowAmount:0f);
		if(isSplashDamage)
		{
			Critter[] critters = critterContainer.GetComponentsInChildren<Critter>();
			foreach(Critter critter in critters.Where(x => x != Target && Vector3.Distance(transform.position,x.transform.position) <= splashDamageRadius))
				ShotTriggered(critter,splashDamageModifier*damage,slowAmount);
		}
		shotTaken = true;
	}

	/// <summary>
	/// Triggers a shot by calling <see cref="ApplyDamage( float damage, float slowMod)"/> on the parameter critter.
	/// </summary>
	/// <param name="critter">Critter.</param>
	/// <param name="damage">Damage.</param>
	/// <param name="slowAmount">Slow amount.</param>
	void ShotTriggered(Critter critter, float damage, float slowAmount)
	{
		critter.ApplyDamage(damage,slowAmount);
	}
}
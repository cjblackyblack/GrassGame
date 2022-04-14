using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
	public Vector2 Input;

	public int Button1Buffer;
	public int Button2Buffer;
	public int Button3Buffer;
	public int Button4Buffer;

	public int Button1ReleaseBuffer;
	public int Button2ReleaseBuffer;
	public int Button3ReleaseBuffer;
	public int Button4ReleaseBuffer;


	public bool Button1Hold;
	public bool Button2Hold;
	public bool Button3Hold;
	public bool Button4Hold;

	public virtual void BeforeObjectUpdate()
	{

	}

	public virtual void PollForTargets()
	{

	}
}
public class Tile {

	private bool _isIndestructible;
	private bool _isUpgrade;
	private bool _isDestructible;
	private UpgradeType _upgradeType;

	public float x;
	public float y;
	public bool isIndestructible {
		get { return _isIndestructible; } 
		set { 
			_isIndestructible = value;
			if (value) {
				_isUpgrade = false;
				_isDestructible = false;
				upgradeType = UpgradeType.None;
			}
		}
	}
	public bool isUpgrade { 
		get { return _isUpgrade; }
		set { 
			_isUpgrade = value;
			if (value) {
				_isIndestructible = false;
			} else {
				_upgradeType = UpgradeType.None;
			}
		} 
	}
	public bool isDestructible { 
		get { return _isDestructible; } 
		set { 
			_isDestructible = value;
			if (value)
				_isIndestructible = false;
		}
	}
	public UpgradeType upgradeType { 
		get { return _upgradeType; } 
		set { 
			_upgradeType = value;
			if (_upgradeType != UpgradeType.None) {
				_isIndestructible = false;
				_isUpgrade = true;
			}
		}
	}

	public Tile(){
		x = 0.0f;
		y = 0.0f;
		_isIndestructible = false;
		_isDestructible = false;
		_isUpgrade = false;
		_upgradeType = UpgradeType.None;
	}
}

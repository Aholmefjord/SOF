public class PlayerAvatar {
	public int id;
	public Avatar avatar;
	public AvatarSkin avatarSkin;
	public int avatarId;
	public int skinId;
	public int eyeId;

	public PlayerAvatar()
	{
		id = 0;
		avatar = new Avatar();
		avatarSkin = new AvatarSkin();
		avatarId = 0;
		skinId = 0;
		eyeId = 0;
	}
}

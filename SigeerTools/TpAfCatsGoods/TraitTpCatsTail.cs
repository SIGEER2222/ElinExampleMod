public class TraitTpCatsTail : TraitToolRangeCane {
	public override bool Contains(RecipeSource r) => true;

	public override bool IsLightOn => true;

	public override void OnCreate(int lv) {
		while ((owner.sockets?.Count ?? 0) < 6) {
			owner.AddSocket();
		}
	}
}

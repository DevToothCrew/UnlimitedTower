mergeInto(LibraryManager.library, {

  SignUp : function() {
    ReactUnityWebGL.SignUp();
  },

  Gacha : function() {
    ReactUnityWebGL.Gacha();
  },

  Login : function() {
    ReactUnityWebGL.Login();
  },

   Logout : function() {
    ReactUnityWebGL.Logout();
  },

  SetFormation : function(formation) {
    ReactUnityWebGL.SetFormation(Pointer_stringify(formation));
  },

  GetStageInfo : function(stage_num) {
    ReactUnityWebGL.GetStageInfo(stage_num);
  },

  BattleAction : function(battleAction) {
    ReactUnityWebGL.BattleAction(Pointer_stringify(battleAction));
  },

  StartBattle : function(battleStart) {
    ReactUnityWebGL.StartBattle(Pointer_stringify(battleStart));
  },

  GetReward : function() {
    ReactUnityWebGL.GetReward();
  },

  GetServant : function() {
	ReactUnityWebGL.GetServant();
  },

  GetItem : function() {
	ReactUnityWebGL.GetItem();
  },

  GetMonster : function() {
	ReactUnityWebGL.GetMonster();
  }

});
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

  BattleAction : function(turn) {
    ReactUnityWebGL.BattleAction(turn));
  },

  StartBattle : function(battleStart) {
    ReactUnityWebGL.StartBattle(Pointer_stringify(battleStart));
  },

  GetReward : function() {
    ReactUnityWebGL.GetReward();
  },

  ExitBattle : function() {
    ReactUnityWebGL.ExitBattle();
  },

  GetBattle : function() {
    ReactUnityWebGL.GetBattle();
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
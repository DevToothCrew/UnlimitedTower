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

  SetFormation : function(a, b, c) {
    ReactUnityWebGL.SetFormation(a, b, c);
  },

  GetStageInfo : function(stage_num) {
    ReactUnityWebGL.GetStageInfo(stage_num);
  },

  BattleAction : function(target_1, action_1, target_2, action_2) {
    ReactUnityWebGL.BattleAction(target_1, action_1, target_2, action_2);
  },

  StartBattle : function(stage_num, party_num) {
    ReactUnityWebGL.StartBattle(stage_num, party_num);
  }

});
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

  SetFormation : function(party_num, pos_1, pos_2, pos_3, pos_4, pos_5, pos_6, pos_7, pos_8, pos_9, pos_10) {
    ReactUnityWebGL.SetFormation(party_num, pos_1, pos_2, pos_3, pos_4, pos_5, pos_6, pos_7, pos_8, pos_9, pos_10);
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
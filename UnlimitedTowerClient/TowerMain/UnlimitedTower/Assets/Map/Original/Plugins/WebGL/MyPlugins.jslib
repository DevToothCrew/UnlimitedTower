mergeInto(LibraryManager.library, {

  SendPacket : function(packet) {
    ReactUnityWebGL.SendPacket(Pointer_stringify(packet));
  }

});
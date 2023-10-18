mergeInto(LibraryManager.library, {

GetLanguage : function(){
	var lang = ysdk.environment.i18n.lang;
	var bufferSize = lengthBytesUTF8(lang) + 1;
	var buffer = _malloc(bufferSize);
	stringToUTF8(lang, buffer, bufferSize);
	return buffer;
},

GetDevice : function(){
	var device = ysdk.deviceInfo.type;
	var bufferSize = lengthBytesUTF8(device) + 1;
	var buffer = _malloc(bufferSize);
	stringToUTF8(device, buffer, bufferSize);
	return buffer;
},

});
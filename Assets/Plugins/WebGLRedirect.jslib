mergeInto(LibraryManager.library, {
  RedirectSameTab_Internal: function (urlPtr) {
    var url = UTF8ToString(urlPtr);
    window.location.href = url;
  }
});

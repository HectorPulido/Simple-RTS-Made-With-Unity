#include <stdlib.h>
#include <string.h>

extern "C" {

  const char *unity_services_current_language_code() {
      NSLocale *locale = [NSLocale currentLocale];
      unsigned long len = locale.languageCode.length;
      char *locale_str = 0;
      locale_str = (char *)malloc(len + 1);
      
      for (unsigned long i = 0; i < len; ++i) {
          char c = [locale.languageCode characterAtIndex:i];
          locale_str[i] = c;
      }
      locale_str[len] = 0;
      
      return locale_str;
  }
}
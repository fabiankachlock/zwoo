export class ReCaptchaTermsVisibilityManager {
  private static instance = new ReCaptchaTermsVisibilityManager();

  private getBadgeElement: () => Element | null = () => null;

  private shouldShow = true;

  static init = (): void => {
    ReCaptchaTermsVisibilityManager.instance.getBadgeElement = () => document.querySelector('.grecaptcha-badge');
  };

  static updateState = (): void => {
    if (this.instance.shouldShow) {
      ReCaptchaTermsVisibilityManager.showBadge();
      return;
    }
    ReCaptchaTermsVisibilityManager.hideBadge();
  };

  static showBadge = (): void => {
    this.instance.shouldShow = true;
    ReCaptchaTermsVisibilityManager.instance.getBadgeElement()?.classList.add('visible');
  };

  static hideBadge = (): void => {
    this.instance.shouldShow = false;
    ReCaptchaTermsVisibilityManager.instance.getBadgeElement()?.classList.remove('visible');
  };
}

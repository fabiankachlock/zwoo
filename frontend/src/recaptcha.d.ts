type GoogleReCaptchaExecuteOptions = {
	action: 'homepage' | 'login' | 'social' | 'e-commerce';
};

interface GoogleReCaptcha {
	ready(callback: () => void): void;
	execute(siteKey: string, options: GoogleReCaptchaExecuteOptions): Promise<string>;
}

export declare global {
	// eslint-disable-next-line
	declare var grecaptcha: GoogleReCaptcha;
	interface Window {
		grecaptcha: GoogleReCaptcha;
	}
}


export type ContactForm = {
  name: string;
  email: string;
  message: string;
  acceptedTerms: boolean;
  acceptedTermsAt: number;
  captchaToken: string;
  site: string;
};

import { WatchStopHandle } from 'vue';

import { ZRPFeedback } from '@/core/domain/zrp/zrpTypes';

import { FeedbackConsumerReason } from './feedbackConfig';
import { useFeedback } from './useFeedback';

export class FeedbackWatcher {
  private readonly reason: FeedbackConsumerReason;
  private stopWatcher: WatchStopHandle | undefined;

  constructor(reason: FeedbackConsumerReason) {
    this.reason = reason;
    this.stopWatcher = undefined;
    this.restart();
  }

  public stop() {
    if (this.stopWatcher) {
      this.stopWatcher();
      this.stopWatcher = undefined;
    }
  }

  public restart() {
    if (this.stopWatcher) {
      this.stop();
    }
    this.stopWatcher = useFeedback(this.reason, this.eventHandler);
  }

  private eventHandler = (feedback: ZRPFeedback) => {
    this._msgHandler(feedback);
  };

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  private _msgHandler: (feedback: ZRPFeedback) => void = () => {};
  public onMessage(handler: (feedback: ZRPFeedback) => void) {
    this._msgHandler = handler;
  }
}

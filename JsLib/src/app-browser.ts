// import { App } from './app';

export class AppBrowser {
    public AnimationRequest: any = null;
    


    public HTMLWindow(): { InnerWidth: number; InnerHeight: number } {
        return { InnerWidth: window.innerWidth, InnerHeight: window.innerHeight };
    }

    public Initialize(): void {
        this.AnimationRequest = null;
        console.info(`Initialize`);
    }

    public Finalize(): void {
        console.info(`Finalize`);
        this.StopAnimation();
    }

    private RenderJS(self: any) {
        console.info(`RenderJS`);
        
        try {
            //self.blazorDotNetObject?.invokeMethodAsync('RenderFrameEventCalled');
            console.info(`RenderJS call JSCallbackTest`,self);
            DotNet.invokeMethodAsync('FoundryBlazor', 'JSCallbackTest','Hello from JS');
            console.info(`RenderJS AfterCall JSCallbackTest`,self);
            
        } catch (error) {
            console.error(`RenderJS ${error}`);
            
        }
        self.AnimationRequest = window.requestAnimationFrame(() => self.RenderJS(self) );
        // request another animation frame
    }

    public StartAnimation() {
        console.info(`StartAnimation`);
        if (this.AnimationRequest == null)
            this.AnimationRequest = window.requestAnimationFrame(() => this.RenderJS(this));
    }

    public StopAnimation() {
        console.info(`StopAnimation`);
        if (this.AnimationRequest != null) 
            window.cancelAnimationFrame(this.AnimationRequest);

        this.AnimationRequest = null;
    }
}

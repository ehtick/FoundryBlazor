// import { App } from './app';

export class AppBrowser {
    public AnimationRequest: any = null;
    public blazorDotNetObject: any = null;

    public SetDotNetObjectReference(ref: any) {
        this.blazorDotNetObject = ref;
    }

    public CopyText(text: string) {
        navigator.clipboard.writeText(text).then(
            () => {
                const message = `Successfully Copied ${text}`;
                console.log(`CopyText ${message}`);
                this.blazorDotNetObject.invokeMethodAsync('OnCopySuccess', message);
            },
            () => {
                const message = `Error: Could not copy ${text}`;
                console.error(`CopyText ${message}`);
                this.blazorDotNetObject.invokeMethodAsync('OnCopyError', message);
            }
        );
    }
    public BoundingClientRect(elementId: string) {
        const node = document.getElementById(elementId);
        if (Boolean(node)) {
            const boundingBox = node.getBoundingClientRect();
            this.blazorDotNetObject.invokeMethodAsync('OnBoundingClientRect', boundingBox);
        } else {
            this.blazorDotNetObject.invokeMethodAsync('OnBoundingClientRect', null);
        }
    }
    public HTMLWindow(): { InnerWidth: number; InnerHeight: number } {
        return { InnerWidth: window.innerWidth, InnerHeight: window.innerHeight };
    }

    public Initialize(ref: any): void {
        this.SetDotNetObjectReference(ref);
    }

    public Finalize(): void {
        if (this.blazorDotNetObject != null)
        {
            this.StopAnimation();
            //var object = this.blazorDotNetObject;
            this.blazorDotNetObject = null;
            //object.dispose();
        }
    }

    private RenderJS(self: any) {
        // Call the blazor component's [JSInvokable] RenderInBlazor method
        if ( self.blazorDotNetObject != null)
        {
            self.AnimationRequest = window.requestAnimationFrame(() => 
            {
                if ( self.blazorDotNetObject != null)
                    self.RenderJS(self)
            });

            try {
                self.blazorDotNetObject?.invokeMethodAsync('RenderFrameEventCalled');
            } catch (error) {
                console.error(`RenderJS ${error}`);
                
            }
            // request another animation frame
        }
    }
    public StartAnimation() {
        if (this.AnimationRequest == null)
            this.AnimationRequest = window.requestAnimationFrame(() => {
                this.RenderJS(this);
            });
    }
    public StopAnimation() {
        if (this.AnimationRequest != null) 
            window.cancelAnimationFrame(this.AnimationRequest);

        this.AnimationRequest = null;
    }
}

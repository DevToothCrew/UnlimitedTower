import * as React from "react";
import IUnityProps from "../interfaces/IUnityProps";
import IUnityState from "../interfaces/IUnityState";
import "../Types";
export default class Unity extends React.Component<IUnityProps, IUnityState> {
    /**
     * Reference to the wrappers html element.
     * @type {HTMLElement}
     * @private
     */
    private htmlElement?;
    /**
     * The Unity content passed by the props.
     * @type {UnityContent}
     * @private
     */
    private unityContent;
    /**
     * The Unity Loader service instance.
     * @type {UnityLoaderService}
     * @private
     */
    private unityLoaderService;
    /**
     * The component state.
     * @type {IUnityState}
     * @public
     */
    state: IUnityState;
    /**
     * The stored binding for the on window
     * resize listener.
     */
    private onWindowResizeBinding;
    /**
     * Initialized the component.
     * @param {IUnityProps} props
     */
    constructor(props: IUnityProps);
    /**
     * An event that is triggered by the Unity player. This tracks
     * the loading progression of the player. It will send '1' when
     * the loading is completed.
     * @param {UnityInstance} unityInstance
     * @param {number} progression
     * @private
     */
    private onProgress;
    /**
     * When the window is resized.
     */
    private onWindowResize;
    /**
     * Since the Unity canvas itself does not respond to the resizing
     * of it's container we have to manually do this. A width and height
     * of 100% does not seem to work, so we have to fetch it's parent's
     * size to adject the canvas.
     * @private
     */
    private adjustCanvasToContainer;
    /**
     * Initialzied the Unity player when the component is mounted.
     * @public
     */
    componentDidMount(): void;
    /**
     * Will remove event listeners and clean up systems when the
     * component is about to unmount.
     * @public
     */
    componentWillUnmount(): void;
    /**
     * Renders the unity wrapper and player.
     * @returns {React.ReactNode} element
     * @public
     */
    render(): React.ReactNode;
}
//# sourceMappingURL=Unity.d.ts.map
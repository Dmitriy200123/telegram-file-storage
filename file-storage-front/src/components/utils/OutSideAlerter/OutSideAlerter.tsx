import React, {RefObject, useEffect, useRef} from "react";


function useOutsideAlerter(ref:RefObject<HTMLInputElement>, onOutsideClick:() => void) {
    useEffect(() => {
        const handleClickOutside = (event: any) =>  {
            if (ref.current && !ref.current.contains(event.target)) {
                // @ts-ignore
                onOutsideClick();
            }
        }

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [ref]);
}


export const OutsideAlerter = (props: { className?:string, onOutsideClick: () => void, children?: any }) => {
    const wrapperRef = useRef(null);
    useOutsideAlerter(wrapperRef, props.onOutsideClick);
    return <div ref={wrapperRef} className={props.className}>{props.children}</div>;
}
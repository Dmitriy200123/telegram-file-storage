import React from "react";
import styles from "./Button.module.css";
import "./Button.scss";
import CSS from 'csstype';

type Props = {
    className?: string | null,
    style?: CSS.Properties,
    type?: "transparent" | "danger" | "white",
    onClick?: () => void
}

export const Button: React.FC<Props> = ({
                                            children,
                                            className, style, type,
    onClick
                                        }) => {
    return <button onClick={onClick} style={style} className={styles.button + " " + (className ? className : "") + (type ? " " + type : "")}>{children}</button>
}


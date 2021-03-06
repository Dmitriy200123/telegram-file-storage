import React from "react";
import styles from "./Button.module.css";
import "./Button.scss";
import CSS from 'csstype';

interface Props extends Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, "type"> {
    style?: CSS.Properties,
    type?: "transparent" | "danger" | "white",
    onClick?: () => void
}

export const Button: React.FC<Props> = ({
                                            children,
                                            className, style, type,
                                            onClick, disabled, ...props
                                        }) => {
    return <button onClick={onClick} style={style} disabled={disabled}
                   className={styles.button + " " + (className ?? "") + (type ? ` ${type}` : "") + (disabled ? ` ${styles.disabled}` : "")}
                   {...props}>
        {children}
    </button>
}


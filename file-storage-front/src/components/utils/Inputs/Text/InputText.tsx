import React from "react";
import styles from "./InputText.module.css";

export const InputText: React.FC<any> = (props) => {
    return <input type={"text"}   {...props} {...props.form} className={styles.input + " " + props.className} />
}

type PropsType = {
    className?: string
}

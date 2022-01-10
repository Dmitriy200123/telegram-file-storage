import React from "react";
import styles from "./InputText.module.css";

export const InputText: React.FC<any> = (props) => {
    return <input type={"text"} className={styles.input + " " + props.className}  {...props} {...props.form} />
}

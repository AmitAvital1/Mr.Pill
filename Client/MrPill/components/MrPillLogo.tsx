import React from "react";
import { Image } from "react-native";
import { PopButton } from "./PopButton";

export function MrPillLogo() {
  const logo = () => {
    return (
      <Image
        source={require("@/assets/images/icon.png")}
        style={{ alignSelf: "center", marginTop: 20 }}
      />
    );
  };

  return <PopButton ButtonContent={logo} BackgroundColor="white" />;
}

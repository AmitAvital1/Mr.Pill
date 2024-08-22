import React, {FunctionComponent} from 'react';
import {View, StyleSheet, Pressable} from 'react-native';

type AppHomeButtonProps = {
  ButtonContent: FunctionComponent;
  ButtonAction?: Function;
  BackgroundColor?: string;
  BorderColor?: string;
  BorderWidth?: number;
};

export const AppHomeButton: React.FC<AppHomeButtonProps> = ({ ButtonContent, ButtonAction, BackgroundColor, BorderColor, BorderWidth }) => {

  if (!BackgroundColor) BackgroundColor = "#c9c9ff";
  if (!BorderColor) BorderColor = "#8a8aa7";
  if (!BorderWidth) BorderWidth = 3;

  const handleOnPress = () => {

    ButtonAction? ButtonAction() : {};
    
  };

  return (
    <Pressable onPress={handleOnPress}>
     <View style={[style.button,{backgroundColor: BackgroundColor, borderColor: BorderColor, borderWidth: BorderWidth}]}>
          <ButtonContent />
      </View>
    </Pressable>
  );
};

const style = StyleSheet.create({  
  button: {
    minWidth: 150,
    flex: 1,
    borderRadius: 1000,
    justifyContent: "center",
    alignItems: "center",
    margin: 10,
  },
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    margin: 20,
  },
});
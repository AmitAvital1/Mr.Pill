import React, {FunctionComponent} from 'react';
import {View, StyleSheet, Pressable} from 'react-native';

type AppHomeButtonProps = {
  ButtonContent: FunctionComponent;
  ButtonAction?: Function;
  BackgroundColor?: string;
};

export const AppHomeButton: React.FC<AppHomeButtonProps> = ({ ButtonContent, ButtonAction }) => {


  const handleOnPress = () => {

    ButtonAction? ButtonAction() : {};
    
  };

  return (
    <Pressable onPress={handleOnPress}>
     <View style={style.button}>
          <ButtonContent />
      </View>
    </Pressable>
  );
};

const style = StyleSheet.create({  
  button: {
    minWidth: 150,
    backgroundColor: "#6666FC",
    flex: 1,
    borderRadius: 1000,
    justifyContent: "center",
    alignItems: "center",
    margin: 10,
    borderColor: "black",
  },
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    margin: 20,
  },
});
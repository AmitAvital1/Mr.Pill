import React, {Component, ComponentElement, FunctionComponent, PureComponent, useEffect, useRef} from 'react';
import {View, StyleSheet, Pressable, Animated} from 'react-native';

type PopButtonProps = {
  ButtonContent: FunctionComponent;
  ButtonAction?: Function;
  BackgroundColor?: string;
};

export const PopButton: React.FC<PopButtonProps> = ({ ButtonContent, ButtonAction }) => {
  const animatedScale = useRef(new Animated.Value(0)).current;

  useEffect(() => {
    animatedScale.setValue(1);
  }, []);

  const handleOnPress = () => {
    animatedScale.setValue(0.8);
    Animated.spring(animatedScale, {
      toValue: 1,
      bounciness: 24,
      speed: 20,
      useNativeDriver: true,
    }).start();
    ButtonAction ? ButtonAction() : {};
  };

  return (
    <View style={style.container}>
      <Pressable onPress={handleOnPress}>
        <Animated.View
          style={[style.button, {transform: [{scale: animatedScale,}]}]}>
          <ButtonContent />
        </Animated.View>
      </Pressable>
    </View>
  );
};

const style = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    margin: 20,
  },
  button: {
    justifyContent: 'center',
    alignItems: 'center',
  },
});
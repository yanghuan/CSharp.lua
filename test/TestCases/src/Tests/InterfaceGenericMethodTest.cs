using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCases {
  public class InterfaceGenericMethodTest {


    public interface IMessageRegister {
      // 注册只接收的消息
      void RegisterMessage<T>(int messageId);
      // 注册即发送，也接收的消息
      void RegisterMessage<TSend, TRecv>(int messageId);
    }
    public class MessageRegister {
      public static void Register(IMessageRegister register) {
        register.RegisterMessage<ClassA, ClassA>(1);
        register.RegisterMessage<ClassA>(2);
      }
    }
  }
}

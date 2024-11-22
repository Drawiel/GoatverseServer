using static GoatverseService.ServiceImplementation;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GoatverseService {
    [ServiceContract]
    public interface ICardsManager {
        [OperationContract]
        List<CardData> ServiceGetAllCards();

        [OperationContract]
        CardData ServiceGetCardById(int id);

        [OperationContract]
        bool ServiceAddCard(CardData cardData);

        [OperationContract]
        bool ServiceDeleteCard(int id);
    }


    [DataContract]
    public class CardData {
        [DataMember]
        public int IdCard { get; set; }

        [DataMember]
        public string CardName { get; set; }

        [DataMember]
        public int Points { get; set; }

        [DataMember]
        public string CardType { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string EffectDescription { get; set; }

        [DataMember]
        public int ImageCardId { get; set; }
    }

}
